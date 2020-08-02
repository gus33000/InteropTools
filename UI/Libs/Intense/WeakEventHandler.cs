using Intense.Presentation;
using Intense.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

namespace Intense
{
    /// <summary>
    /// A generic event handler which allows the event target to be garbage collected when there are no other references to it.
    /// </summary>
    /// <typeparam name="TEventTarget"></typeparam>
    /// <typeparam name="TEventTypedSource"></typeparam>
    /// <typeparam name="TEventSource"></typeparam>
    /// <typeparam name="TEventArgs"></typeparam>
    internal class WeakEventHandler<TEventTarget, TEventTypedSource, TEventSource, TEventArgs>
        where TEventTarget : class
        where TEventTypedSource : TEventSource
    {
        private WeakReference<TEventTarget> reference;

        /// <summary>
        /// Initializes a new instance of the <see cref="WeakEventHandler{TEventTarget, TEventTypedSource, TEventSource, TEventArgs}"/>.
        /// </summary>
        /// <param name="target"></param>
        public WeakEventHandler(TEventTarget target)
        {
            this.reference = new WeakReference<TEventTarget>(target);
        }

        /// <summary>
        /// Handles the event and forwards it to the event target if it's still alive.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        public void OnEvent(TEventSource source, TEventArgs args)
        {
            if (this.reference == null) {
                return;
            }

            TEventTarget target;
            if (this.reference.TryGetTarget(out target)) {
                Handle(target, source, args);
            }
            else {
                Detach(this, (TEventTypedSource)source);

                this.reference = null;
                this.Handle = null;
                this.Detach = null;
            }
        }

        /// <summary>
        /// The event handler of the target.
        /// </summary>
        public Action<TEventTarget, TEventSource, TEventArgs> Handle { get; set; }
        /// <summary>
        /// The method for detaching the event handler.
        /// </summary>
        public Action<WeakEventHandler<TEventTarget, TEventTypedSource, TEventSource, TEventArgs>, TEventTypedSource> Detach { get; set; }
    }
}
