using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using Serilog;

namespace Aesha.Infrastructure
{
    /// <summary>
    /// Aggregate base class, which factors out some common infrastructure that
    /// all aggregates have (ID and event application).
    /// </summary>
    public class Aggregate
    {
        private readonly MethodInfo _applyOneEventMethodInfo;

        public Aggregate()
        {
            _applyOneEventMethodInfo = GetType().GetMethod(nameof(ApplyOneEvent));
        }

        /// <summary>
        /// The number of events loaded into this aggregate.
        /// </summary>
        public int EventsLoaded { get; set; }

        /// <summary>
        /// The unique ID of the aggregate.
        /// </summary>
        public string Id { get; protected set; }

        /// <summary>
        /// Enuerates the supplied events and applies them in order to the aggregate.
        /// </summary>
        /// <param name="events"></param>
        /// <param name="logger"></param>
        public void ApplyEvents(IEnumerable events, ILogger logger = null)
        {
            foreach (var e in events)
            {
                ApplyEvent(logger, e);
            }
        }

        private void ApplyEvent(ILogger logger, object e)
        {
            try
            {
                var method = _applyOneEventMethodInfo.MakeGenericMethod(e.GetType());
                method.Invoke(this, new[] {e});
            }
            catch (Exception ex)
            {
                logger?.Warning(ex, "Error occured whilst applying event");
            }
        }

        /// <summary>
        /// Applies a single event to the aggregate.
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="ev"></param>
        public void ApplyOneEvent<TEvent>(TEvent ev)
        {
            var applier = GetType()
                .GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public)
                .SingleOrDefault(m => m.GetParameters().SingleOrDefault(e => e.ParameterType == ev.GetType()) != null);
            
            //var applier = this as IApplyEvent<TEvent>;
            if (applier == null)
            {
                throw new InvalidOperationException($"Aggregate {GetType().Name} does not know how to apply event {ev.GetType().Name}");
            }
            
            applier.Invoke(this, new object[] {ev});

            EventsLoaded++;
        }
    }
}
