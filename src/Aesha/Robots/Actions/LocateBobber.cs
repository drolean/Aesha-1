using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Aesha.Core;
using Aesha.Domain;
using Aesha.Infrastructure;

namespace Aesha.Robots.Actions
{
    public class LocateBobber : IConditionalAction
    {
        private Point _bobberLocation;

        public LocateBobber()
        {
            _bobberLocation = Point.Empty;
        }

        public bool Evaluate()
        {
            return _bobberLocation == Point.Empty;
        }

        public void Do()
        {
            if (_bobberLocation != Point.Empty) return;

            for (var y = 200; y <= 700; y += 30)
            {
                for (var x = 800; x <= 1000; x += 20)
                {
                    var mouseOverUnitGuid = GetPositionMouseOverUnit(new Point(x, y));
                    var itemFound = ObjectManager.Objects.FirstOrDefault(obj => obj.Guid == mouseOverUnitGuid);

                    var gameObject = itemFound as WowGameObject;
                    if (gameObject != null)
                    {
                        _bobberLocation = new Point(x, y);
                        while (!Task.Delay(Timings.ThirtySeconds).IsCompleted)
                        {
                            if (gameObject.Bobbing == 1)
                            {
                                KeyboardCommandDispatcher.GetKeyboard().SendShiftClick(_bobberLocation);
                                Task.Delay(500).Wait();
                                _bobberLocation = Point.Empty;
                                return;
                            }
                            
                        }
                    }
                }
            }
        }

        private ulong GetPositionMouseOverUnit(Point point)
        {
            var offsetPoint = KeyboardCommandDispatcher.GetKeyboard().GetOffsetPoint(point);
            Cursor.Position = offsetPoint;
            Task.Delay(5).Wait();
            return CommandManager.GetDefault().MouseOverUnit;
        }
    }
}