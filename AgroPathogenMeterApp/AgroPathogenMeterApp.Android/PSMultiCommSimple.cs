using Microsoft.AppCenter.Crashes;
using PalmSens;
using PalmSens.Comm;
using PalmSens.Devices;
using PalmSens.Plottables;
using PalmSens.Techniques;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AgroPathogenMeterApp.Droid
{
    public partial class PSMultiCommSimple
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PSCommSimple" /> class.
        /// This class handles is used to perform measurements and control a collection of channels manually.
        /// It requires a reference to the platform specific instance of the class,
        /// i.e. PSMultiCommSimpleWinForms, PSMultiCommSimpleWPF or PSMultiCommSimpleXamarin
        /// </summary>
        /// <param name="platform">The reference to the platform specific PSMultiCommSimple class.</param>
        /// <exception cref="System.ArgumentNullException">Platform cannot be null</exception>
        public PSMultiCommSimple(IPlatformMulti platform)
        {
            _platform = platform ?? throw new ArgumentNullException("Platform cannot be null");
        }

        #region Properties

        /// <summary>
        /// The platform specific interface for WinForms, WPF and Xamarin support
        /// </summary>
        private IPlatformMulti _platform = null;

        /// <summary>
        /// The connected channels' CommManagers
        /// </summary>
        private CommManager[] _comms;

        /// <summary>
        /// Gets or sets the connected channels' CommManagers and (un)subscribes the corresponding events.
        /// </summary>
        /// <value>
        /// The CommManager.
        /// </value>
        public CommManager[] Comms
        {
            get { return _comms; }
            set
            {
                if (_comms != null) //Unsubscribe events
                {
                    foreach (CommManager comm in _comms)
                        if (comm != null)
                            UnSubscribeCommEvents(comm);
                }
                _comms = value;
                if (_comms != null) //Subscribe events
                {
                    foreach (CommManager comm in _comms)
                        if (comm != null)
                            SubscribeCommEvents(comm);
                }
            }
        }

        /// <summary>
        /// The task completion source used to obtain the active measurement in the Measure and MeasureAsync functions
        /// </summary>
        private Dictionary<CommManager, TaskCompletionSource<SimpleMeasurement>> _taskCompletionSources = new Dictionary<CommManager, TaskCompletionSource<SimpleMeasurement>>();

        /// <summary>
        /// Gets a value indicating whether <see cref="PSCommSimple"/> is connected to any channels.
        /// </summary>
        /// <value>
        ///   <c>true</c> if connected; otherwise, <c>false</c>.
        /// </value>
        public bool Connected { get { return Comms != null; } }

        /// <summary>
        /// Gets the number of connected channels.
        /// </summary>
        /// <value>
        /// The n connected channels.
        /// </value>
        public int NConnectedChannels { get { return _comms == null ? 0 : _comms.Length; } }

        /// <summary>
        /// Gets the connected devices.
        /// </summary>
        /// <value>
        /// The connected devices.
        /// </value>
        /// <exception cref="System.NullReferenceException">Not connected to any channels.</exception>
        public Device[] ConnectedDevices
        {
            get
            {
                if (_comms == null)
                    throw new NullReferenceException("Not connected to any channels.");
                return _comms.Select(c => c.Device).ToArray();
            }
        }

        /// <summary>
        /// Gets the connected channel types.
        /// </summary>
        /// <value>
        /// The connected channel types.
        /// </value>
        /// <exception cref="System.NullReferenceException">Not connected to any channels.</exception>
        public enumDeviceType[] ConnectedChannels
        {
            get
            {
                if (_comms == null)
                    throw new NullReferenceException("Not connected to any channels.");
                return _comms.Select(c => c.DeviceType).ToArray();
            }
        }

        /// <summary>
        /// Gets the states of the connected channels.
        /// </summary>
        /// <value>
        /// The states of the connected channels.
        /// </value>
        /// <exception cref="System.NullReferenceException">Not connected to any channels.</exception>
        public CommManager.DeviceState[] ChannelStates
        {
            get
            {
                if (_comms == null)
                    throw new NullReferenceException("Not connected to any channels.");
                return _comms.Select(c => c.State).ToArray();
            }
        }

        /// <summary>
        /// Gets values indicating whether the connected channels' [cell is on].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [cell is on]; otherwise, <c>false</c>.
        /// </value>
        /// <exception cref="System.NullReferenceException">Not connected to any channels.</exception>
        public bool[] IsCellOn
        {
            get
            {
                if (_comms == null)
                    throw new NullReferenceException("Not connected to any channels.");
                return _comms.Select(c => c.CellOn).ToArray();
            }
        }

        /// <summary>
        /// Gets the capabilities of the connected channels.
        /// </summary>
        /// <value>
        /// The channel capabilities.
        /// </value>
        /// <exception cref="System.NullReferenceException">Not connected to any channels.</exception>
        public DeviceCapabilities[] Capabilities
        {
            get
            {
                if (_comms == null)
                    throw new NullReferenceException("Not connected to any channels.");
                return _comms.Select(c => c.Capabilities).ToArray();
            }
        }

        /// <summary>
        /// Determines whether [the specified method] is compatible with all connected channels.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <returns>
        ///   <c>true</c> if the method is valid; otherwise, <c>false</c>.
        /// </returns>
        public bool[] IsValidMethod(Method method)
        {
            ValidateMethod(method, out bool[] valid, out List<string>[] errors);
            return valid;
        }

        /// <summary>
        /// Determines whether [the specified method] is compatible with the specified channel.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <param name="channel">The specified channel.</param>
        /// <returns>
        ///   <c>true</c> if the method is valid; otherwise, <c>false</c>.
        /// </returns>
        public bool IsValidMethod(Method method, int channel)
        {
            ValidateMethod(method, channel, out bool valid, out List<string> errors);
            return valid;
        }

        /// <summary>
        /// Determines whether [the specified method] is compatible with the specified channels.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <param name="channels">The specified channels.</param>
        /// <returns>
        ///   <c>true</c> if the method is valid; otherwise, <c>false</c>.
        /// </returns>
        public bool[] IsValidMethod(Method method, int[] channels)
        {
            ValidateMethod(method, channels, out bool[] valid, out List<string>[] errors);
            return valid;
        }

        /// <summary>
        /// The active SimpleMeasurements
        /// </summary>
        private Dictionary<CommManager, SimpleMeasurement> _activeSimpleMeasurements = new Dictionary<CommManager, SimpleMeasurement>();

        /// <summary>
        /// Gets array with indices of all channels.
        /// </summary>
        /// <value>
        /// All channels.
        /// </value>
        /// <exception cref="System.NullReferenceException">Not connected to any channels.</exception>
        private int[] AllChannels
        {
            get
            {
                if (_comms == null)
                    throw new NullReferenceException("Not connected to any channels.");
                int n = _comms.Length;
                int[] channels = new int[n];
                for (int i = 0; i < n; i++)
                    channels[i] = i;
                return channels;
            }
        }

        #endregion Properties

        #region Functions

        /// <summary>
        /// Subscribes to the CommManager's events.
        /// </summary>
        /// <param name="comm">The comm.</param>
        private void SubscribeCommEvents(CommManager comm)
        {
            comm.BeginMeasurementAsync += _comm_BeginMeasurementAsync;
            comm.EndMeasurementAsync += _comm_EndMeasurementAsync;
            comm.BeginReceiveCurve += _comm_BeginReceiveCurve;
            comm.ReceiveStatusAsync += _comm_ReceiveStatusAsync;
            comm.StateChangedAsync += _comm_StateChangedAsync;
            comm.Disconnected += _comm_Disconnected;
            comm.CommErrorOccurred += _comm_CommErrorOccurred;
        }

        /// <summary>
        /// Unsubscribes the CommManager's events.
        /// </summary>
        /// <param name="comm">The comm.</param>
        private void UnSubscribeCommEvents(CommManager comm)
        {
            comm.BeginMeasurementAsync -= _comm_BeginMeasurementAsync;
            comm.EndMeasurementAsync -= _comm_EndMeasurementAsync;
            comm.BeginReceiveCurve -= _comm_BeginReceiveCurve;
            comm.ReceiveStatusAsync -= _comm_ReceiveStatusAsync;
            comm.StateChangedAsync -= _comm_StateChangedAsync;
            comm.Disconnected -= _comm_Disconnected;
            comm.CommErrorOccurred -= _comm_CommErrorOccurred;
        }

        /// <summary>
        /// Disconnects from the connected channels.
        /// </summary>
        /// <exception cref="System.NullReferenceException">Not connected to any channels.</exception>
        public async Task Disconnect()
        {
            try { await _platform.Disconnect(_comms); }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                throw new NullReferenceException("Not connected to any channels.");
            }
            Comms = null;
            _activeSimpleMeasurements.Clear();
        }

        /// <summary>
        /// Runs a measurement as specified in the method on the specified channel.
        /// </summary>
        /// <param name="method">The method containing the measurement parameters.</param>
        /// <param name="channel">The channel.</param>
        /// <param name="muxChannel">The mux channel to measure on.</param>
        /// <returns>
        /// A SimpleMeasurement instance containing all the data related to the measurement.
        /// </returns>
        /// <exception cref="System.NullReferenceException">
        /// Not connected to any channels.
        /// or
        /// Not connected to specified channel.
        /// </exception>
        /// <exception cref="System.ArgumentException">Method is incompatible with the connected channel.</exception>
        /// <exception cref="System.Exception">Could not start measurement.</exception>
        public async Task<SimpleMeasurement> MeasureAsync(Method method, int channel, int muxChannel, TaskBarrier taskBarrier = null)
        {
            if (_comms == null)
                throw new NullReferenceException("Not connected to any channels.");

            CommManager comm = _comms[channel];

            if (comm == null)
                throw new NullReferenceException("Not connected to specified channel.");
            if (comm.State != CommManager.DeviceState.Idle)
                throw new ArgumentException("Measurements can only be started on idle channels");

            //Create a copy of the method and update the method with the device's supported current ranges
            Method copy = null;
            Method.CopyMethod(method, ref copy);
            copy.Ranging.SupportedCurrentRanges = Capabilities[channel].SupportedRanges;

            //Check whether method is compatible with the connected channel
            ValidateMethod(copy, channel, out bool isValidMethod, out List<string> errors);
            if (!isValidMethod)
                throw new ArgumentException("Method is incompatible with the connected channel.");

            _taskCompletionSources[comm] = new TaskCompletionSource<SimpleMeasurement>();
            AsyncEventHandler<ActiveMeasurement> asyncEventHandler = new AsyncEventHandler<ActiveMeasurement>(async (object sender, ActiveMeasurement m) =>
            {
                CommManager commSender = sender as CommManager;
                if (commSender.ChannelIndex != channel) return;

                SimpleMeasurement simpleMeasurement = new SimpleMeasurement(m);
                simpleMeasurement.Title += $" Channel {commSender.ChannelIndex + 1}";
                simpleMeasurement.Channel = commSender.ChannelIndex + 1;
                _activeSimpleMeasurements.Add(commSender, simpleMeasurement);
                if (m is ImpedimetricMeasurement eis)
                    _activeSimpleMeasurements[commSender].NewSimpleCurve(PalmSens.Data.DataArrayType.ZRe, PalmSens.Data.DataArrayType.ZIm, "Nyquist", true); //Create a nyquist curve by default

                _taskCompletionSources[commSender].SetResult(simpleMeasurement);
            });
            comm.BeginMeasurementAsync += asyncEventHandler;

            //Start the measurement on the connected channel, this triggers an event that updates _activeMeasurement
            string error = await RunAsync(async () =>
            {
                //Need to check again as the task can be scheduled to run at a later point after which this could have changed
                if (comm == null)
                    throw new NullReferenceException("Not connected to specified channel.");
                return await comm.MeasureAsync(copy, muxChannel, taskBarrier);
            }, comm);
            if (!(string.IsNullOrEmpty(error)))
                throw new Exception($"Could not start measurement: {error}");

            SimpleMeasurement result = await _taskCompletionSources[comm].Task;
            comm.BeginMeasurementAsync -= asyncEventHandler;

            return result;
        }

        /// <summary>
        /// Runs a measurement as specified in the method on the specified collection of channels.
        /// </summary>
        /// <param name="method">The method containing the measurement parameters.</param>
        /// <param name="channels">The channels.</param>
        /// <param name="muxChannel">The mux channel to measure on.</param>
        /// <returns>
        /// A SimpleMeasurement instance containing all the data related to the measurement.
        /// </returns>
        /// <exception cref="System.NullReferenceException">Not connected to any channels.
        /// or
        /// Not connected to specified channel.</exception>
        /// <exception cref="System.ArgumentException">Method is incompatible with the connected channel.</exception>
        /// <exception cref="System.Exception">Could not start measurement.</exception>
        public async Task<SimpleMeasurement[]> MeasureAsync(Method method, int[] channels, int muxChannel)
        {
            int n = channels.Length;
            //Barrier used to synchronize measurements (measurements initiate first and desynchronize slightly
            //before the measurement is triggered on the channels, this barrier synchornizes the triggering on the channels)
            TaskBarrier taskBarrier = new TaskBarrier(n);
            List<Task<SimpleMeasurement>> measurementTasks = new List<Task<SimpleMeasurement>>();

            for (int i = 0; i < n; i++)
                measurementTasks.Add(MeasureAsync(method, i, muxChannel, taskBarrier));

            return await Task.WhenAll(measurementTasks);
        }

        /// <summary>
        /// Runs a measurement as specified in the method on all channels.
        /// </summary>
        /// <param name="method">The method containing the measurement parameters.</param>
        /// <param name="channels">The channels.</param>
        /// <param name="muxChannel">The mux channel to measure on.</param>
        /// <returns>
        /// A SimpleMeasurement instance containing all the data related to the measurement.
        /// </returns>
        /// <exception cref="System.NullReferenceException">Not connected to any channels.
        /// or
        /// Not connected to specified channel.</exception>
        /// <exception cref="System.ArgumentException">Method is incompatible with the connected channel.</exception>
        /// <exception cref="System.Exception">Could not start measurement.</exception>
        public async Task<SimpleMeasurement[]> MeasureAllChannelsAsync(Method method, int muxChannel)
        {
            return await MeasureAsync(method, AllChannels, muxChannel);
        }

        /// <summary>
        /// Runs a measurement as specified in the method on the specified channel.
        /// </summary>
        /// <param name="method">The method containing the measurement parameters.</param>
        /// <returns>A SimpleMeasurement instance containing all the data related to the measurement.</returns>
        public async Task<SimpleMeasurement> MeasureAsync(Method method, int channel)
        {
            if (method.MuxMethod == MuxMethod.Sequentially)
                return await MeasureAsync(method, channel, method.GetNextSelectedMuxChannel(-1));
            else
                return await MeasureAsync(method, channel, -1);
        }

        /// <summary>
        /// Runs a measurement as specified in the method on the specified channel.
        /// </summary>
        /// <param name="method">The method containing the measurement parameters.</param>
        /// <param name="channels">The channels.</param>
        /// <returns>
        /// A SimpleMeasurement instance containing all the data related to the measurement.
        /// </returns>
        public async Task<SimpleMeasurement[]> MeasureAsync(Method method, int[] channels)
        {
            if (method.MuxMethod == MuxMethod.Sequentially)
                return await MeasureAsync(method, channels, method.GetNextSelectedMuxChannel(-1));
            else
                return await MeasureAsync(method, channels, -1);
        }

        /// <summary>
        /// Runs a measurement as specified in the method on the specified channel.
        /// </summary>
        /// <param name="method">The method containing the measurement parameters.</param>
        /// <param name="channels">The channels.</param>
        /// <returns>
        /// A SimpleMeasurement instance containing all the data related to the measurement.
        /// </returns>
        public async Task<SimpleMeasurement[]> MeasureAllChannelsAsync(Method method)
        {
            if (method.MuxMethod == MuxMethod.Sequentially)
                return await MeasureAsync(method, AllChannels, method.GetNextSelectedMuxChannel(-1));
            else
                return await MeasureAsync(method, AllChannels, -1);
        }

        /// <summary>
        /// Aborts the active measurement on the specified channel.
        /// </summary>
        /// <param name="comm">The comm.</param>
        /// <returns></returns>
        /// <exception cref="System.NullReferenceException">Not connected to specified channel.</exception>
        private async Task AbortMeasurementAsync(CommManager comm)
        {
            if (comm == null)
                throw new NullReferenceException("Not connected to specified channel.");
            if (comm.ActiveMeasurement == null)
                throw new Exception("Device is not measuring.");

            if (comm.ActiveMeasurement != null)
                await RunAsync(async () =>
                {
                    //Need to check again as the task can be scheduled to run at a later point after which this could have changed
                    if (comm == null)
                        throw new NullReferenceException("Not connected to specified channel.");
                    if (comm.ActiveMeasurement == null)
                        throw new Exception("Device is not measuring.");
                    await comm.AbortAsync();
                }, comm);
        }

        /// <summary>
        /// Aborts the active measurement on the specified channel.
        /// </summary>
        /// <exception cref="System.NullReferenceException">Not connected to a channel.</exception>
        /// <exception cref="System.Exception">The channel is not currently performing measurement</exception>
        public async Task AbortMeasurementAsync(int channel)
        {
            if (_comms == null)
                throw new NullReferenceException("Not connected to any channels.");

            await AbortMeasurementAsync(_comms[channel]);
        }

        /// <summary>
        /// Aborts the active measurement on the specified channels.
        /// </summary>
        /// <exception cref="System.NullReferenceException">Not connected to a channel.</exception>
        /// <exception cref="System.Exception">The channel is not currently performing measurement</exception>
        public async Task AbortMeasurementsAsync(int[] channels)
        {
            List<Task> abortTasks = new List<Task>();
            foreach (int channel in channels)
                if (_comms[channel].ActiveMeasurement != null)
                    abortTasks.Add(AbortMeasurementAsync(channel));

            await Task.WhenAll(abortTasks);
        }

        /// <summary>
        /// Aborts all active measurements channels.
        /// </summary>
        /// <exception cref="System.NullReferenceException">Not connected to a channel.</exception>
        /// <exception cref="System.Exception">The channel is not currently performing measurement</exception>
        public async Task AbortAllActiveMeasurementsAsync()
        {
            List<Task> abortTasks = new List<Task>();
            foreach (CommManager comm in _activeSimpleMeasurements.Keys)
                if (comm.ActiveMeasurement != null)
                    abortTasks.Add(AbortMeasurementAsync(comm));

            await Task.WhenAll(abortTasks);
        }

        /// <summary>
        /// Turns the cell on on specified channel.
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <returns></returns>
        /// <exception cref="System.NullReferenceException">Not connected to a channel</exception>
        /// <exception cref="System.Exception">Channel must be in idle mode for manual control</exception>
        public async Task TurnCellOnAsync(int channel)
        {
            if (_comms == null)
                throw new NullReferenceException("Not connected to any channels.");

            CommManager comm = _comms[channel];

            if (comm == null)
                throw new NullReferenceException("Not connected to a channel");
            if (comm.State != CommManager.DeviceState.Idle)
                throw new Exception("Channel must be in idle mode for manual control");
            if (comm.CellOn)
                return;

            await RunAsync(async () =>
            {
                //Need to check again as the task can be scheduled to run at a later point after which this could have changed
                if (comm == null)
                    throw new NullReferenceException("Not connected to a channel");
                if (comm.State != CommManager.DeviceState.Idle)
                    throw new Exception("Channel must be in idle mode for manual control");
                if (comm.CellOn)
                    return;
                await comm.SetCellOnAsync(true);
            }, comm);
        }

        /// <summary>
        /// Turns the cell on on specified channels.
        /// </summary>
        /// <param name="channels">The channels.</param>
        /// <returns></returns>
        /// <exception cref="System.NullReferenceException">Not connected to a channel</exception>
        /// <exception cref="System.Exception">Channel must be in idle mode for manual control</exception>
        public async Task TurnCellOnAsync(int[] channels)
        {
            List<Task> cellOnTasks = new List<Task>();
            foreach (int channel in channels)
                cellOnTasks.Add(TurnCellOnAsync(channel));
            await Task.WhenAll(cellOnTasks);
        }

        /// <summary>
        /// Turns the cell on on all channels.
        /// </summary>
        /// <exception cref="System.NullReferenceException">Not connected to a channel</exception>
        /// <exception cref="System.Exception">Channel must be in idle mode for manual control</exception>
        public async Task TurnCellOnAsync()
        {
            await TurnCellOnAsync(AllChannels);
        }

        /// <summary>
        /// Turns the cell off on specified channel.
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <returns></returns>
        /// <exception cref="System.NullReferenceException">Not connected to a channel</exception>
        /// <exception cref="System.Exception">Channel must be in idle mode for manual control</exception>
        public async Task TurnCellOffAsync(int channel)
        {
            if (_comms == null)
                throw new NullReferenceException("Not connected to any channels.");

            CommManager comm = _comms[channel];

            if (comm == null)
                throw new NullReferenceException("Not connected to a channel");
            if (comm.State != CommManager.DeviceState.Idle)
                throw new Exception("Channel must be in idle mode for manual control");
            if (!comm.CellOn)
                return;

            await RunAsync(async () =>
            {
                //Need to check again as the task can be scheduled to run at a later point after which this could have changed
                if (comm == null)
                    throw new NullReferenceException("Not connected to a channel");
                if (comm.State != CommManager.DeviceState.Idle)
                    throw new Exception("Channel must be in idle mode for manual control");
                if (!comm.CellOn)
                    return;
                await comm.SetCellOnAsync(false);
            }, comm);
        }

        /// <summary>
        /// Turns the cell off on specified channels.
        /// </summary>
        /// <param name="channels">The channels.</param>
        /// <returns></returns>
        /// <exception cref="System.NullReferenceException">Not connected to a channel</exception>
        /// <exception cref="System.Exception">Channel must be in idle mode for manual control</exception>
        public async Task TurnCellOffAsync(int[] channels)
        {
            List<Task> cellOffTasks = new List<Task>();
            foreach (int channel in channels)
                cellOffTasks.Add(TurnCellOffAsync(channel));
            await Task.WhenAll(cellOffTasks);
        }

        /// <summary>
        /// Turns the cell off on all channels.
        /// </summary>
        /// <exception cref="System.NullReferenceException">Not connected to a channel</exception>
        /// <exception cref="System.Exception">Channel must be in idle mode for manual control</exception>
        public async Task TurnCellOffAsync()
        {
            await TurnCellOffAsync(AllChannels);
        }

        /// <summary>
        /// Sets the cell potential on the specified channel.
        /// </summary>
        /// <param name="potential">The potential.</param>
        /// <param name="channel">The channel.</param>
        /// <returns></returns>
        /// <exception cref="System.NullReferenceException">Not connected to a channel</exception>
        /// <exception cref="System.Exception">Channel must be in idle mode for manual control</exception>
        public async Task SetCellPotentialAsync(float potential, int channel)
        {
            if (_comms == null)
                throw new NullReferenceException("Not connected to any channels.");

            CommManager comm = _comms[channel];

            if (comm == null)
                throw new NullReferenceException("Not connected to a channel");
            if (await comm.GetStateAsync() != CommManager.DeviceState.Idle)
                throw new Exception("Channel must be in idle mode for manual control");

            await RunAsync(async () =>
            {
                //Need to check again as the task can be scheduled to run at a later point after which this could have changed
                if (comm == null)
                    throw new NullReferenceException("Not connected to a channel");
                if (await comm.GetStateAsync() != CommManager.DeviceState.Idle)
                    throw new Exception("Channel must be in idle mode for manual control");
                await comm.SetPotentialAsync(potential);
            }, comm);
        }

        /// <summary>
        /// Sets the cell potential on the specified channels.
        /// </summary>
        /// <param name="potential">The potential.</param>
        /// <param name="channel">The channel.</param>
        /// <returns></returns>
        /// <exception cref="System.NullReferenceException">Not connected to a channel</exception>
        /// <exception cref="System.Exception">Channel must be in idle mode for manual control</exception>
        public async Task SetCellPotentialAsync(float potential, int[] channels)
        {
            List<Task> setCellPotentialTasks = new List<Task>();
            foreach (int channel in channels)
                setCellPotentialTasks.Add(SetCellPotentialAsync(potential, channel));
            await Task.WhenAll(setCellPotentialTasks);
        }

        /// <summary>
        /// Sets the cell potential on all channels.
        /// </summary>
        /// <param name="potential">The potential.</param>
        /// <param name="channel">The channel.</param>
        /// <returns></returns>
        /// <exception cref="System.NullReferenceException">Not connected to a channel</exception>
        /// <exception cref="System.Exception">Channel must be in idle mode for manual control</exception>
        public async Task SetCellPotentialAsync(float potential)
        {
            await SetCellPotentialAsync(potential, AllChannels);
        }

        /// <summary>
        /// Reads the cell potential on the specified channel.
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <returns></returns>
        /// <exception cref="System.NullReferenceException">Not connected to any channels.
        /// or
        /// Not connected to a channel</exception>
        /// <exception cref="System.Exception">Channel must be in idle mode for manual control</exception>
        /// <exception cref="NullReferenceException">Not connected to a channel</exception>
        /// <exception cref="Exception">Channel must be in idle mode for manual control</exception>
        public async Task<float> ReadCellPotentialAsync(int channel)
        {
            if (_comms == null)
                throw new NullReferenceException("Not connected to any channels.");

            CommManager comm = _comms[channel];

            if (comm == null)
                throw new NullReferenceException("Not connected to a channel");
            if (comm.State != CommManager.DeviceState.Idle)
                throw new Exception("Channel must be in idle mode for manual control");

            return await RunAsync<float>(async () =>
            {
                //Need to check again as the task can be scheduled to run at a later point after which this could have changed
                if (comm == null)
                    throw new NullReferenceException("Not connected to a channel");
                if (comm.State != CommManager.DeviceState.Idle)
                    throw new Exception("Channel must be in idle mode for manual control");
                return await comm.GetPotentialAsync();
            }, comm);
        }

        /// <summary>
        /// Reads the cell potential on the specified channels.
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <returns></returns>
        /// <exception cref="System.NullReferenceException">Not connected to any channels.
        /// or
        /// Not connected to a channel</exception>
        /// <exception cref="System.Exception">Channel must be in idle mode for manual control</exception>
        /// <exception cref="NullReferenceException">Not connected to a channel</exception>
        /// <exception cref="Exception">Channel must be in idle mode for manual control</exception>
        public async Task<float[]> ReadCellPotentialAsync(int[] channels)
        {
            List<Task<float>> readCellPotentialTasks = new List<Task<float>>();
            foreach (int channel in channels)
                readCellPotentialTasks.Add(ReadCellPotentialAsync(channel));
            return await Task.WhenAll(readCellPotentialTasks);
        }

        /// <summary>
        /// Reads the cell potential on all channels.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.NullReferenceException">Not connected to any channels.
        /// or
        /// Not connected to a channel</exception>
        /// <exception cref="System.Exception">Channel must be in idle mode for manual control</exception>
        /// <exception cref="NullReferenceException">Not connected to a channel</exception>
        /// <exception cref="Exception">Channel must be in idle mode for manual control</exception>
        public async Task<float[]> ReadCellPotentialAsync()
        {
            return await ReadCellPotentialAsync(AllChannels);
        }

        /// <summary>
        /// Sets the cell current on the specified channel.
        /// </summary>
        /// <param name="current">The current.</param>
        /// <param name="channel">The channel.</param>
        /// <returns></returns>
        /// <exception cref="System.NullReferenceException">Not connected to a channel</exception>
        /// <exception cref="System.Exception">Channel must be in idle mode for manual control</exception>
        public async Task SetCellCurrentAsync(float current, int channel)
        {
            if (_comms == null)
                throw new NullReferenceException("Not connected to any channels.");

            CommManager comm = _comms[channel];

            if (comm == null)
                throw new NullReferenceException("Not connected to a channel");
            if (await comm.GetStateAsync() != CommManager.DeviceState.Idle)
                throw new Exception("Channel must be in idle mode for manual control");
            if (!comm.Capabilities.IsGalvanostat)
                throw new Exception("Channel does not support Galvanostat mode");

            await RunAsync(async () =>
            {
                //Need to check again as the task can be scheduled to run at a later point after which this could have changed
                if (comm == null)
                    throw new NullReferenceException("Not connected to a channel");
                if (await comm.GetStateAsync() != CommManager.DeviceState.Idle)
                    throw new Exception("Channel must be in idle mode for manual control");
                if (!comm.Capabilities.IsGalvanostat)
                    throw new Exception("Channel does not support Galvanostat mode");
                await comm.SetCurrentAsync(current);
            }, comm);
        }

        /// <summary>
        /// Sets the cell current on the specified channels.
        /// </summary>
        /// <param name="current">The current.</param>
        /// <param name="channel">The channel.</param>
        /// <returns></returns>
        /// <exception cref="System.NullReferenceException">Not connected to a channel</exception>
        /// <exception cref="System.Exception">Channel must be in idle mode for manual control</exception>
        public async Task SetCellCurrentAsync(float current, int[] channels)
        {
            List<Task> setCellCurrentTasks = new List<Task>();
            foreach (int channel in channels)
                setCellCurrentTasks.Add(SetCellCurrentAsync(current, channel));
            await Task.WhenAll(setCellCurrentTasks);
        }

        /// <summary>
        /// Sets the cell current on all channels.
        /// </summary>
        /// <param name="current">The current.</param>
        /// <param name="channel">The channel.</param>
        /// <returns></returns>
        /// <exception cref="System.NullReferenceException">Not connected to a channel</exception>
        /// <exception cref="System.Exception">Channel must be in idle mode for manual control</exception>
        public async Task SetCellCurrentAsync(float current)
        {
            await SetCellCurrentAsync(current, AllChannels);
        }

        /// <summary>
        /// Reads the cell current on the specified channel.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NullReferenceException">Not connected to a channel</exception>
        /// <exception cref="Exception">Channel must be in idle mode for manual control</exception>
        public async Task<float> ReadCellCurrentAsync(int channel)
        {
            if (_comms == null)
                throw new NullReferenceException("Not connected to any channels.");

            CommManager comm = _comms[channel];

            if (comm == null)
                throw new NullReferenceException("Not connected to a channel");
            if (comm.State != CommManager.DeviceState.Idle)
                throw new Exception("Channel must be in idle mode for manual control");

            return await RunAsync<float>(async () =>
            {
                //Need to check again as the task can be scheduled to run at a later point after which this could have changed
                if (comm == null)
                    throw new NullReferenceException("Not connected to a channel");
                if (comm.State != CommManager.DeviceState.Idle)
                    throw new Exception("Channel must be in idle mode for manual control");
                return await comm.GetCurrentAsync();
            }, comm);
        }

        /// <summary>
        /// Reads the cell current on the specified channels.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NullReferenceException">Not connected to a channel</exception>
        /// <exception cref="Exception">Channel must be in idle mode for manual control</exception>
        public async Task<float[]> ReadCellCurrentAsync(int[] channels)
        {
            List<Task<float>> readCellCurrentTasks = new List<Task<float>>();
            foreach (int channel in channels)
                readCellCurrentTasks.Add(ReadCellCurrentAsync(channel));
            return await Task.WhenAll(readCellCurrentTasks);
        }

        /// <summary>
        /// Reads the cell current on all channels.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NullReferenceException">Not connected to a channel</exception>
        /// <exception cref="Exception">Channel must be in idle mode for manual control</exception>
        public async Task<float[]> ReadCellCurrentAsync()
        {
            return await ReadCellCurrentAsync(AllChannels);
        }

        /// <summary>
        /// Sets the current range on the specified channel.
        /// </summary>
        /// <param name="currentRange">The current range.</param>
        /// <exception cref="System.NullReferenceException">Not connected to a channel</exception>
        /// <exception cref="System.Exception">Channel must be in idle mode for manual control</exception>
        public async Task SetCurrentRangeAsync(CurrentRange currentRange, int channel)
        {
            if (_comms == null)
                throw new NullReferenceException("Not connected to any channels.");

            CommManager comm = _comms[channel];

            if (comm == null)
                throw new NullReferenceException("Not connected to a channel");
            if (await comm.GetStateAsync() != CommManager.DeviceState.Idle)
                throw new Exception("Channel must be in idle mode for manual control");

            await RunAsync(async () =>
            {
                //Need to check again as the task can be scheduled to run at a later point after which this could have changed
                if (comm == null)
                    throw new NullReferenceException("Not connected to a channel");
                if (await comm.GetStateAsync() != CommManager.DeviceState.Idle)
                    throw new Exception("Channel must be in idle mode for manual control");
                await comm.SetCurrentRangeAsync(currentRange);
            }, comm);
        }

        /// <summary>
        /// Sets the current range on the specified channels.
        /// </summary>
        /// <param name="currentRange">The current range.</param>
        /// <exception cref="System.NullReferenceException">Not connected to a channel</exception>
        /// <exception cref="System.Exception">Channel must be in idle mode for manual control</exception>
        public async Task SetCurrentRangeAsync(CurrentRange currentRange, int[] channels)
        {
            List<Task> setCurrentRangesTasks = new List<Task>();
            foreach (int channel in channels)
                setCurrentRangesTasks.Add(SetCurrentRangeAsync(currentRange, channel));
            await Task.WhenAll(setCurrentRangesTasks);
        }

        /// <summary>
        /// Sets the current range on all channels.
        /// </summary>
        /// <param name="currentRange">The current range.</param>
        /// <exception cref="System.NullReferenceException">Not connected to a channel</exception>
        /// <exception cref="System.Exception">Channel must be in idle mode for manual control</exception>
        public async Task SetCurrentRangeAsync(CurrentRange currentRange)
        {
            await SetCurrentRangeAsync(currentRange, AllChannels);
        }

        /// <summary>
        /// Validates whether the specified method is compatible with the capabilities of the specified connected channel.
        /// </summary>
        /// <param name="method">The method containing the measurement parameters.</param>
        /// <param name="channel">The specified channel.</param>
        /// <param name="isValidMethod">if set to <c>true</c> [is valid method].</param>
        /// <param name="errors">The errors.</param>
        /// <exception cref="System.NullReferenceException">Not connected to a channel.</exception>
        /// <exception cref="System.ArgumentNullException">The specified method cannot be null.</exception>
        public void ValidateMethod(Method method, int channel, out bool isValidMethod, out List<string> errors)
        {
            if (_comms == null)
                throw new NullReferenceException("Not connected to any channel.");
            if (method == null)
                throw new ArgumentNullException("The specified method cannot be null.");
            errors = new List<string>();

            //Get a list of method compatability warnings and errors for the connected channel
            List<MethodError> methodErrors = method.Validate(_comms[channel].Capabilities);

            //Check wheteher the channel can perform the measurement described in the method
            isValidMethod = !(methodErrors.Where(c => c.IsFatal == true).Any());

            //Build a list of the warnings and errors
            foreach (MethodError error in methodErrors)
                errors.Add($"{error.Parameter.ToString()}: {error.Message}");
        }

        /// <summary>
        /// Validates whether the specified method is compatible with the capabilities of the specified connected channels.
        /// </summary>
        /// <param name="method">The method containing the measurement parameters.</param>
        /// <param name="channels">The specified channels.</param>
        /// <param name="isValidMethod">if set to <c>true</c> [is valid method].</param>
        /// <param name="errors">The errors.</param>
        /// <exception cref="System.NullReferenceException">Not connected to a channel.</exception>
        /// <exception cref="System.ArgumentNullException">The specified method cannot be null.</exception>
        public void ValidateMethod(Method method, int[] channels, out bool[] isValidMethod, out List<string>[] errors)
        {
            int n = channels.Length;
            isValidMethod = new bool[n];
            errors = new List<string>[n];

            for (int i = 0; i < n; i++)
            {
                ValidateMethod(method, channels[i], out bool valid, out List<string> error);
                isValidMethod[i] = valid;
                errors[i] = error;
            }
        }

        /// <summary>
        /// Validates whether the specified method is compatible with the capabilities of all connected channels.
        /// </summary>
        /// <param name="method">The method containing the measurement parameters.</param>
        /// <param name="isValidMethod">if set to <c>true</c> [is valid method].</param>
        /// <param name="errors">The errors.</param>
        /// <exception cref="System.NullReferenceException">Not connected to a channel.</exception>
        /// <exception cref="System.ArgumentNullException">The specified method cannot be null.</exception>
        public void ValidateMethod(Method method, out bool[] isValidMethod, out List<string>[] errors)
        {
            ValidateMethod(method, AllChannels, out isValidMethod, out errors);
        }

        /// <summary>
        /// Adds the active curve and its respective to the collection and subscribes to its events.
        /// </summary>
        /// <param name="activeCurve">The active curve.</param>
        private void OnSimpleCurveStartReceivingData(Curve activeCurve, CommManager comm)
        {
            if (activeCurve == null)
                return;

            SimpleMeasurement activeSimpleMeasurement = _activeSimpleMeasurements[comm];
            SimpleCurve activeSimpleCurve = activeSimpleMeasurement.SimpleCurveCollection.Where(sc => sc.Curve == activeCurve).FirstOrDefault();

            if (activeSimpleCurve == null)
            {
                activeSimpleCurve = new SimpleCurve(activeCurve, activeSimpleMeasurement);
                activeSimpleMeasurement.AddSimpleCurve(activeSimpleCurve);
            }

            SimpleCurveStartReceivingData?.Invoke(this, activeSimpleCurve);
        }

        /// <summary>
        /// Runs an async Func delegate asynchronously on the clientconnections taskscheduler.
        /// </summary>
        /// <param name="func">The action.</param>
        /// <param name="comm">The connection to run the delegate on.</param>
        /// <returns></returns>
        private async Task RunAsync(Func<Task> func, CommManager comm)
        {
            await new SynchronizationContextRemover();
            await comm.ClientConnection.RunAsync(func);
        }

        /// <summary>
        /// Runs an async Func delegate asynchronously on the clientconnections taskscheduler.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func">The action.</param>
        /// <param name="comm">The connection to run the delegate on.</param>
        /// <returns></returns>
        private async Task<T> RunAsync<T>(Func<Task<T>> func, CommManager comm)
        {
            await new SynchronizationContextRemover();
            return await comm.ClientConnection.RunAsync(func);
        }

        #endregion Functions

        #region events

        /// <summary>
        /// Occurs when a channel status package is received, these packages are not sent during a measurement.
        /// </summary>
        public event MultiChannelStatusEventHandler ReceiveStatus;

        /// <summary>
        /// Casts ReceiveStatus events coming from a different thread to the UI thread when necessary.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="StatusEventArgs" /> instance containing the channel status.</param>
        /// <exception cref="System.NullReferenceException">Platform not set.</exception>
        private async Task _comm_ReceiveStatusAsync(object sender, StatusEventArgs e)
        {
            if (_platform == null)
                throw new NullReferenceException("Platform not set.");
            if (_platform.InvokeIfRequired(new AsyncEventHandler<StatusEventArgs>(_comm_ReceiveStatusAsync), sender, e)) //Recast event to UI thread when necessary
                return;
            CommManager comm = Comms.Where(c => c.ClientConnection == sender).First();
            if (comm == null)
                throw new NullReferenceException("Sender no longer in connected Comms list");
            ReceiveStatus?.Invoke(this, e, comm.ChannelIndex);
        }

        /// <summary>
        /// Occurs at the start of a new measurement.
        /// </summary>
        public event MultiChannelEventHandler MeasurementStarted;

        /// <summary>
        /// Sets the ActiveMeasurement at the start of a measurement and casts BeginMeasurement events coming from a different thread to the UI thread when necessary.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The new measurement.</param>
        /// <exception cref="System.NullReferenceException">Platform not set.</exception>
        private async Task _comm_BeginMeasurementAsync(object sender, ActiveMeasurement e)
        {
            if (_platform == null)
                throw new NullReferenceException("Platform not set.");
            if (_platform.InvokeIfRequired(new AsyncEventHandler<ActiveMeasurement>(_comm_BeginMeasurementAsync), sender, e)) //Recast event to UI thread when necessary
                return;
            CommManager comm = sender as CommManager;
            MeasurementStarted?.Invoke(this, comm.ChannelIndex);
        }

        /// <summary>
        /// Occurs when a measurement has ended.
        /// </summary>
        public event MultiChannelEventHandler MeasurementEnded;

        /// <summary>
        /// Sets the ActiveMeasurement to null at the end of the measurement and casts EndMeasurement events coming from a different thread to the UI thread when necessary.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        /// <exception cref="System.NullReferenceException">Platform not set.</exception>
        private async Task _comm_EndMeasurementAsync(object sender, EventArgs e)
        {
            if (_platform == null)
                throw new NullReferenceException("Platform not set.");
            if (_platform.InvokeIfRequired(new AsyncEventHandler(_comm_EndMeasurementAsync), sender, e)) //Recast event to UI thread when necessary
                return;
            CommManager comm = sender as CommManager;
            _activeSimpleMeasurements.Remove(comm);
            MeasurementEnded?.Invoke(this, comm.ChannelIndex);
        }

        /// <summary>
        /// Adds the active Curve to the active SimpleMeasurement and casts BeginReceiveCurve events coming from a different thread to the UI thread when necessary.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="CurveEventArgs"/> instance containing the event data.</param>
        /// <exception cref="System.NullReferenceException">Platform not set.</exception>
        private void _comm_BeginReceiveCurve(object sender, CurveEventArgs e)
        {
            if (_platform == null)
                throw new NullReferenceException("Platform not set.");
            if (_platform.InvokeIfRequired(new CurveEventHandler(_comm_BeginReceiveCurve), sender, e)) //Recast event to UI thread when necessary
                return;
            OnSimpleCurveStartReceivingData(e.GetCurve(), sender as CommManager);
        }

        /// <summary>
        /// Occurs when a new [SimpleCurve starts receiving data].
        /// </summary>
        public event PSCommSimple.SimpleCurveStartReceivingDataHandler SimpleCurveStartReceivingData;

        /// <summary>
        /// Occurs when the devive's [state changed].
        /// </summary>
        public event MultiChannelStateChangedEventHandler StateChanged;

        /// <summary>
        /// Casts StateChanged events coming from a different thread to the UI thread when necessary.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">State of the current.</param>
        /// <exception cref="System.NullReferenceException">Platform not set.</exception>
        private async Task _comm_StateChangedAsync(object sender, CommManager.DeviceState e)
        {
            if (_platform == null)
                throw new NullReferenceException("Platform not set.");
            if (_platform.InvokeIfRequired(new AsyncEventHandler<CommManager.DeviceState>(_comm_StateChangedAsync), sender, e)) //Recast event to UI thread when necessary
                return;
            StateChanged?.Invoke(this, e, (sender as CommManager).ChannelIndex);
        }

        /// <summary>
        /// Occurs when a channel is [disconnected].
        /// </summary>
        public event MultiChannelDisconnectedEventHandler Disconnected;

        /// <summary>
        /// Raises the Disconnected event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        private void _comm_Disconnected(object sender, EventArgs e)
        {
            if (_platform == null)
                throw new NullReferenceException("Platform not set.");
            if (_platform.InvokeIfRequired(new EventHandler(_comm_Disconnected), sender, e)) //Recast event to UI thread when necessary
                return;

            CommManager comm = sender as CommManager;
            Exception exception = _commErrorExceptions.ContainsKey(comm) ? _commErrorExceptions[comm] : null;
            Disconnected?.Invoke(this, exception, comm.ChannelIndex);
            if (_commErrorExceptions.ContainsKey(comm))
                _commErrorExceptions.Remove(comm);
        }

        /// <summary>
        /// The latest comm error exception, this is used for the disconnected event and is set back to null directly after it is raised
        /// </summary>
        private Dictionary<CommManager, Exception> _commErrorExceptions = new Dictionary<CommManager, Exception>();

        /// <summary>
        /// Comms the comm error occorred.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        private void _comm_CommErrorOccurred(object sender, Exception exception)
        {
            if (_platform == null)
                throw new NullReferenceException("Platform not set.");
            if (_platform.InvokeIfRequired(new CommManager.EventHandlerCommErrorOccurred(_comm_CommErrorOccurred), exception)) //Recast event to UI thread when necessary
                return;
            _commErrorExceptions.Add(sender as CommManager, exception);
        }

        #endregion events

        public void Dispose()
        {
            if (Connected)
                foreach (CommManager comm in Comms)
                    comm.Dispose();
            _comms = null;
            _activeSimpleMeasurements = null;
            _commErrorExceptions = null;
        }
    }

    #region MultiChannelEventHandlers

    /// <summary>
    /// MultiChannelEventHandler that includes which channel the event was raised by
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="channel">The channel.</param>
    public delegate void MultiChannelEventHandler(object sender, int channel);

    /// <summary>
    /// EventHandler that reports changes in the DeviceState of a channel
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="ChannelState">State of the channel.</param>
    /// <param name="channel">The channel.</param>
    public delegate void MultiChannelStateChangedEventHandler(object sender, CommManager.DeviceState ChannelState, int channel);

    /// <summary>
    /// EventHandler that reports idle status readings of a channel
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="status">The <see cref="StatusEventArgs"/> instance containing the event data.</param>
    /// <param name="channel">The channel.</param>
    public delegate void MultiChannelStatusEventHandler(object sender, StatusEventArgs status, int channel);

    /// <summary>
    /// EventHandler that reports when a channel is disconnected if this was due to an exception this is also included
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="exception">The exception.</param>
    /// <param name="channel">The channel.</param>
    public delegate void MultiChannelDisconnectedEventHandler(object sender, Exception exception, int channel);

    #endregion MultiChannelEventHandlers
}