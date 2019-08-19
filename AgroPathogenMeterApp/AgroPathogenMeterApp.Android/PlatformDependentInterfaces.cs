using PalmSens.Comm;
using System;
using System.Threading.Tasks;

namespace AgroPathogenMeterApp.Droid
{
    public interface IPlatform
    {
        /// <summary>
        /// Invokes if required.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <param name="args">The arguments.</param>
        /// <returns></returns>
        bool InvokeIfRequired(Delegate method, params object[] args);

        void Disconnect(CommManager comm);
    }

    public interface IPlatformMulti
    {
        /// <summary>
        /// Invokes if required.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <param name="args">The arguments.</param>
        /// <returns></returns>
        bool InvokeIfRequired(Delegate method, params object[] args);

        Task Disconnect(CommManager[] comms);
    }

    public interface IPlatformInvoker
    {
        /// <summary>
        /// Invokes if required.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <param name="args">The arguments.</param>
        /// <returns></returns>
        bool InvokeIfRequired(Delegate method, params object[] args);
    }
}