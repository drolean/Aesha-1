using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Aesha.Infrastructure;
using Aesha.Interfaces;
using Binarysharp.Assemblers.Fasm;

namespace Aesha.Core
{
    public static class FasmExtensions
    {
        public static void Inject(this FasmNet fasm, IntPtr processPtr, uint address)
        {
            var assembled = fasm.Assemble();
            var size = assembled.Length;

            var dataPtr = Marshal.AllocHGlobal(Marshal.SizeOf((object) assembled[0])*size);
            Marshal.Copy(assembled, 0, dataPtr, size);

            var bytesWritten = IntPtr.Zero;
            Win32Imports.WriteProcessMemory(processPtr, address, dataPtr, size, out bytesWritten);
        }
    }


    public class InjectionManager : IInjectionManager, IDisposable
    {
        private uint _injectedCode;
        private uint _injectionAddress;
        private uint _returnInjectionCode;
        
        private readonly IProcessMemoryReader _processMemoryReader;
        private readonly IntPtr _processPtr;
        private readonly FasmNet _fasm;

        private const uint StandardRightsRequired = 0x000F0000;
        private const uint Synchronize = 0x00100000;
        private const uint ProcessAllAccess = StandardRightsRequired | Synchronize | 0xFFF;
        
        public InjectionManager(IWowProcess process, IProcessMemoryReader processMemoryReader)
        {
            _processPtr = Win32Imports.OpenProcess(ProcessAllAccess, false, process.Id);
            _processMemoryReader = processMemoryReader;
            _fasm = new FasmNet();
        }

        private uint GetEndScenePointer()
        {
            const uint dxDevice = 0xC5DF88;
            const uint dxDeviceIdx = 0x397C;
            const uint endsceneIdx = 0xA8;

            var devicePtr = _processMemoryReader.ReadUInt(dxDevice);
            var endPtr = _processMemoryReader.ReadUInt(devicePtr + dxDeviceIdx);
            var scenePtr = _processMemoryReader.ReadUInt(endPtr);
            return _processMemoryReader.ReadUInt(scenePtr + endsceneIdx);
        }

        private bool IsHooked()
        {
            return _processMemoryReader.ReadByte(GetEndScenePointer()) == 0xE9;
        }

        private uint AllocateMemory(int size)
        {
            return Win32Imports.VirtualAllocEx(_processPtr, 0, size, 0x00001000 | 0x00002000, 0x40);
        }

        private void FreeMemory(uint address)
        {
            Win32Imports.VirtualFreeEx(_processPtr, address, 0, 0x8000);
        }
        
        private void CreateHook()
        {
            if (IsHooked()) return;

            var endScenePtr = GetEndScenePointer();

            _injectedCode = AllocateMemory(0x2048);

            _injectionAddress = AllocateMemory(0x4);
            _processMemoryReader.WriteInt(_injectionAddress, 0);

            _returnInjectionCode = AllocateMemory(0x4);
            _processMemoryReader.WriteInt(_returnInjectionCode, 0);

            _fasm.Clear();
            _fasm.AddLine("use32");
            _fasm.AddLine("pushad");
            _fasm.AddLine("pushfd");
            _fasm.AddLine("mov eax, [" + _injectionAddress + "]");
            _fasm.AddLine("test eax, eax");
            _fasm.AddLine("je @out");
            _fasm.AddLine("mov eax, [" + _injectionAddress + "]");
            _fasm.AddLine("call eax");
            _fasm.AddLine("mov [" + _returnInjectionCode + "], eax");
            _fasm.AddLine("mov edx, " + _injectionAddress);
            _fasm.AddLine("mov ecx, 0");
            _fasm.AddLine("mov [edx], ecx");
            _fasm.AddLine("@out:");
            _fasm.AddLine("popfd");
            _fasm.AddLine("popad");

            var sizeAsm = (uint) _fasm.Assemble().Length;
            _fasm.Inject(_processPtr, _injectedCode);
            
            _fasm.Clear();
            _fasm.AddLine("mov edi, edi");
            _fasm.AddLine("push ebp");
            _fasm.AddLine("mov ebp, esp");
            _fasm.Inject(_processPtr, _injectedCode + sizeAsm);

            _fasm.Clear();
            _fasm.AddLine("jmp " + (endScenePtr + 5));
            _fasm.Inject(_processPtr, _injectedCode + sizeAsm + (uint) 5);

            _fasm.Clear();
            _fasm.AddLine("jmp " + (_injectedCode));
            _fasm.Inject(_processPtr, endScenePtr);
        }

        private void DisposeHooking()
        { 
                var endScenePtr = GetEndScenePointer();

                if (IsHooked())
                {
                    _fasm.Clear();
                    _fasm.AddLine("mov edi, edi");
                    _fasm.AddLine("push ebp");
                    _fasm.AddLine("mov ebp, esp");
                    _fasm.Inject(_processPtr, endScenePtr);
                }

                FreeMemory(_injectedCode);
                FreeMemory(_injectionAddress);
                FreeMemory(_returnInjectionCode);
        }

        private void InjectAndExecute(IEnumerable<string> asm)
        {
            if (!IsHooked()) CreateHook();

            _processMemoryReader.WriteInt(_returnInjectionCode, 0);

            _fasm.Clear();
            foreach (string line in asm)
                _fasm.AddLine(line);

            var assembled = _fasm.Assemble();
            var codeCavePtr = AllocateMemory(assembled.Length);
            _fasm.Inject(_processPtr, codeCavePtr);
            _processMemoryReader.WriteInt(_injectionAddress, (int) codeCavePtr);

            while (_processMemoryReader.ReadInt(_injectionAddress) > 0)
                Thread.Sleep(5); 

            FreeMemory(codeCavePtr);
        }

        public void LuaDoString(string command)
        {
            var parameter = Encoding.UTF8.GetBytes(command);
            var allocationPtr = AllocateMemory(parameter.Length + 1);
            _processMemoryReader.WriteBytes(allocationPtr, parameter, parameter.Length);

            var asm = new[]
            {
                "mov EDX, 0",
                "mov ECX, " + allocationPtr,
                "call " + (uint)Offsets.LuaFunctions.DoString,
                "retn",
            };

            InjectAndExecute(asm);
            FreeMemory(allocationPtr);
        }

        public void AutoLoot()
        {
                var asm = new[]
                {
                    "call " + (uint)Offsets.LuaFunctions.AutoLoot,
                    "retn",
                };

                InjectAndExecute(asm);
        }

        public void RightClickObject(uint objectBaseAddress, int autoLoot)
        {
            var asm = new[]
            {
                "push " + autoLoot,
                "mov ECX, " + objectBaseAddress,
                "call " + (uint) Offsets.LuaFunctions.OnRightClickObject,
                "retn",
            };

            InjectAndExecute(asm);
        }

        public void RightClickUnit(uint objectBaseAddress, int autoLoot)
        {
            var asm = new[]
            {
                "push " + autoLoot,
                "mov ECX, " + objectBaseAddress,
                "call " + (uint) Offsets.LuaFunctions.OnRightClickUnit,
                "retn",
            };

            InjectAndExecute(asm);
        }
        

        public void Dispose()
        {
            DisposeHooking();
            GC.SuppressFinalize(this);
        }

        ~InjectionManager()
        {
            DisposeHooking();
        }
    }
}