using Android.Content;
using Android.OS;
using Android.Util;
using Android.Views;
using PalmSens;
using PalmSens.Comm;
using PalmSens.Core.Simplified;
using PalmSens.Core.Simplified.Data;
using PalmSens.Devices;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AgroPathogenMeterApp.Droid
{
    public class PSCommSimpleAndroid : View, IPlatform
    {
        public PSCommSimpleAndroid(Context context, IAttributeSet attrs) :
            base(context, attrs)
        {
            Initialize();
            _context = context;
        }

        public PSCommSimpleAndroid(Context context, IAttributeSet attrs, int defStyle) :
            base(context, attrs, defStyle)
        {
            Initialize();
        }

        private void Initialize()
        {
            this.Visibility = ViewStates.Gone;
            PalmSens.PSAndroid.Utils.CoreDependencies.Init(Context);
            _psCommSimple = new PSCommSimple(this);
            _deviceHandler = new DeviceHandler();
        }

        #region Properties

        /// <summary>
        /// Instance of the platform independent PSCommSimple class that manages measurements and manual control
        /// </summary>
        private PSCommSimple _psCommSimple;

        /// <summary>
        /// The context
        /// </summary>
        private Context _context;

        public Context CurrentContext
        {
            get
            {
                return _context;
            }
            set
            {
                _context = value;
                _deviceHandler.Context = _context;
            }
        }

        /// <summary>
        /// The device handler class which handles the connection to the device
        /// </summary>
        private DeviceHandler _deviceHandler;

        /// <summary>
        /// Gets a value indicating whether <see cref="PSCommSimple"/> is connected to a device.
        /// </summary>
        /// <value>
        ///   <c>true</c> if connected; otherwise, <c>false</c>.
        /// </value>
        public bool Connected
        {
            get { return _psCommSimple.Connected; }
        }

        /// <summary>
        /// Gets the connected device type.
        /// </summary>
        /// <value>
        /// The connected device type.
        /// </value>
        public enumDeviceType ConnectedDevice
        {
            get { return _psCommSimple.ConnectedDevice; }
        }

        /// <summary>
        /// Returns an array of connected devices.
        /// </summary>
        /// <param name="timeOut">Discovery time out in milliseconds.</param>
        /// <returns></returns>
        /// <value>
        /// The connected devices.
        /// </value>
        public async Task<Device[]> GetConnectedDevices(int timeOut = 20000)
        {
            return await _deviceHandler.ScanDevices(timeOut);
        }

        /// <summary>
        /// Gets the state of the connected device.
        /// </summary>
        /// <value>
        /// The state of the device.
        /// </value>
        public CommManager.DeviceState DeviceState
        {
            get { return _psCommSimple.DeviceState; }
        }

        /// <summary>
        /// Gets a value indicating whether [cell is on].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [cell is on]; otherwise, <c>false</c>.
        /// </value>
        public bool CellOn
        {
            get { return _psCommSimple.IsCellOn; }
        }

        /// <summary>
        /// Gets the capabilities of the connected device.
        /// </summary>
        /// <value>
        /// The device capabilities.
        /// </value>
        public DeviceCapabilities Capabilities
        {
            get { return _psCommSimple.Capabilities; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to enable devices connected via bluetooth.
        /// </summary>
        /// <value>
        ///   <c>true</c> Enable scan for devices over bluetooth; Disable scan for devices over bluetooth <c>false</c>.
        /// </value>
        public bool EnableBluetooth
        {
            get { return _deviceHandler.EnableBluetooth; }
            set { _deviceHandler.EnableBluetooth = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to enable devices connected via usb.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [enable usb]; otherwise, <c>false</c>.
        /// </value>
        public bool EnableUSB
        {
            get { return _deviceHandler.EnableUSB; }
            set { _deviceHandler.EnableUSB = value; }
        }

        /// <summary>
        /// Determines whether [the specified method] is compatible with the device.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <returns>
        ///   <c>true</c> if the method is valid; otherwise, <c>false</c>.
        /// </returns>
        public bool IsValidMethod(Method method)
        {
            return _psCommSimple.IsValidMethod(method);
        }

        #endregion Properties

        #region Functions

        /// <summary>
        /// Connects to the specified device.
        /// </summary>
        /// <param name="device">The device.</param>
        public void Connect(Device device)
        {
            _psCommSimple.Comm = _deviceHandler.Connect(device);
        }

        /// <summary>
        /// Disconnects from the connected device.
        /// </summary>
        public void Disconnect()
        {
            _psCommSimple.Disconnect();
        }

        /// <summary>
        /// Turns the cell on.
        /// </summary>
        public void TurnCellOn()
        {
            _psCommSimple.TurnCellOn();
        }

        /// <summary>
        /// Turns the cell off.
        /// </summary>
        public void TurnCellOff()
        {
            _psCommSimple.TurnCellOff();
        }

        /// <summary>
        /// Sets the cell potential.
        /// </summary>
        /// <param name="potential">The potential.</param>
        public void SetCellPotential(float potential)
        {
            _psCommSimple.SetCellPotential(potential);
        }

        /// <summary>
        /// Sets the cell current.
        /// </summary>
        /// <param name="current">The current.</param>
        public void SetCellCurrent(float current)
        {
            _psCommSimple.SetCellCurrent(current);
        }

        /// <summary>
        /// Sets the current range.
        /// </summary>
        /// <param name="currentRange">The current range.</param>
        public void SetCurrentRange(CurrentRange currentRange)
        {
            _psCommSimple.SetCurrentRange(currentRange);
        }

        /// <summary>
        /// Runs a measurement as specified in the method on the connected device.
        /// </summary>
        /// <param name="method">The method containing the measurement parameters.</param>
        /// <param name="muxChannel">The mux channel to measure on.</param>
        /// <returns>
        /// A SimpleMeasurement instance containing all the data related to the measurement.
        /// </returns>
        /// <exception cref="System.NullReferenceException">Not connected to a device.</exception>
        /// <exception cref="System.ArgumentException">Method is incompatible with the connected device.</exception>
        /// <exception cref="System.Exception">Could not start measurement.</exception>
        public SimpleMeasurement Measure(Method method, int muxChannel)
        {
            return _psCommSimple.Measure(method, muxChannel);
        }

        /// <summary>
        /// Runs a measurement as specified in the method on the connected device.
        /// </summary>
        /// <param name="method">The method containing the measurement parameters.</param>
        /// <returns>A SimpleMeasurement instance containing all the data related to the measurement.</returns>
        public SimpleMeasurement Measure(Method method)
        {
            return _psCommSimple.Measure(method);
        }

        /// <summary>
        /// Aborts the active measurement.
        /// </summary>
        public void AbortMeasurement()
        {
            _psCommSimple.AbortMeasurement();
        }

        /// <summary>
        /// Validates whether the specified method is compatible with the capabilities of the connected device.
        /// </summary>
        /// <param name="method">The method containing the measurement parameters.</param>
        /// <param name="isValidMethod">if set to <c>true</c> [is valid method].</param>
        /// <param name="errors">The errors.</param>
        /// <exception cref="System.NullReferenceException">Not connected to a device.</exception>
        /// <exception cref="System.ArgumentNullException">The specified method cannot be null.</exception>
        public void ValidateMethod(Method method, out bool isValidMethod, out List<string> errors)
        {
            _psCommSimple.ValidateMethod(method, out isValidMethod, out errors);
        }

        #endregion Functions

        #region Platform interface

        private Handler mainHandler = new Handler(Looper.MainLooper);

        /// <summary>
        /// Invokes event to UI thread if required.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <param name="args">The arguments.</param>
        /// <returns></returns>
        /// <exception cref="System.NullReferenceException">Parent control not set.</exception>
        public bool InvokeIfRequired(Delegate method, params object[] args)
        {
            if (Looper.MyLooper() != Looper.MainLooper)//Check if event needs to be cast to the UI thread
            {
                mainHandler.Post(() => method.DynamicInvoke(args)); //Recast event to UI thread
                return true;
            }
            return false;
        }

        /// <summary>
        /// Disconnects from device with the specified CommManager.
        /// Warning use the platform independent method Disconnect() instead.
        /// Otherwise the generic PSCommSimple does not unsubscribe from the CommManager correctly
        /// which may result in it not being released from the memory.
        /// </summary>
        /// <param name="comm">The comm.</param>
        public void Disconnect(CommManager comm)
        {
            _deviceHandler.Disconnect(comm);
        }

        #endregion Platform interface

        #region events

        /// <summary>
        /// Occurs when a device status package is received, these packages are not sent during a measurement.
        /// </summary>
        public event StatusEventHandler ReceiveStatus
        {
            add { _psCommSimple.ReceiveStatus += value; }
            remove { _psCommSimple.ReceiveStatus -= value; }
        }

        /// <summary>
        /// Occurs at the start of a new measurement.
        /// </summary>
        public event EventHandler MeasurementStarted
        {
            add { _psCommSimple.MeasurementStarted += value; }
            remove { _psCommSimple.MeasurementStarted -= value; }
        }

        /// <summary>
        /// Occurs when a measurement has ended.
        /// </summary>
        public event EventHandler MeasurementEnded
        {
            add { _psCommSimple.MeasurementEnded += value; }
            remove { _psCommSimple.MeasurementEnded -= value; }
        }

        /// <summary>
        /// Occurs when a new [SimpleCurve starts receiving data].
        /// </summary>
        public event PSCommSimple.SimpleCurveStartReceivingDataHandler SimpleCurveStartReceivingData
        {
            add { _psCommSimple.SimpleCurveStartReceivingData += value; }
            remove { _psCommSimple.SimpleCurveStartReceivingData -= value; }
        }

        /// <summary>
        /// Occurs when the devive's [state changed].
        /// </summary>
        public event CommManager.StatusChangedEventHandler StateChanged
        {
            add { _psCommSimple.StateChanged += value; }
            remove { _psCommSimple.StateChanged -= value; }
        }

        #endregion events
    }
}