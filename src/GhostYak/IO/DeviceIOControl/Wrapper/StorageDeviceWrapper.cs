using GhostYak.IO.DeviceIOControl.Objects.Disk;
using GhostYak.IO.DeviceIOControl.Objects.Enums;
using Microsoft.Win32.SafeHandles;

namespace GhostYak.IO.DeviceIOControl.Wrapper
{
    public class StorageDeviceWrapper : DeviceIoWrapperBase
    {
        public StorageDeviceWrapper(SafeFileHandle handle, bool ownsHandle = false)
            : base(handle, ownsHandle)
        {

        }

        //StorageCheckVerify
        //StorageCheckVerify2
        //StorageMediaRemoval

        /// <summary>
        /// Used to f.ex. open/eject CD Rom trays
        /// <see cref="http://msdn.microsoft.com/en-us/library/windows/desktop/aa363406(v=vs.85).aspx" />
        /// </summary>
        public bool StorageEjectMedia()
        {
            return DeviceIoControlHelper.InvokeIoControl(Handle, IOControlCode.StorageEjectMedia);
        }

        /// <summary>
        /// Used to f.ex. close CD Rom trays
        /// <see cref="http://msdn.microsoft.com/en-us/library/windows/desktop/aa363414(v=vs.85).aspx" />
        /// </summary>
        public bool StorageLoadMedia()
        {
            return DeviceIoControlHelper.InvokeIoControl(Handle, IOControlCode.StorageLoadMedia);
        }

        //StorageLoadMedia2
        //StorageReserve
        //StorageRelease
        //StorageFindNewDevices
        //StorageEjectionControl
        //StorageMcnControl
        //StorageGetMediaTypes
        //StorageGetMediaTypesEx
        //StorageResetBus
        //StorageResetDevice

        public STORAGE_DEVICE_NUMBER StorageGetDeviceNumber()
        {
            return DeviceIoControlHelper.InvokeIoControl<STORAGE_DEVICE_NUMBER>(Handle, IOControlCode.StorageGetDeviceNumber);
        }

        //StoragePredictFailure
        //StorageObsoleteResetBus
        //StorageObsoleteResetDevice

        /// <summary><see cref="https://docs.microsoft.com/ko-kr/windows/desktop/api/winioctl/ni-winioctl-ioctl_storage_query_property"/></summary>
        public STORAGE_DEVICE_DESCRIPTOR StorageQueryProperty()
        {
            STORAGE_PROPERTY_QUERY spq = default;
            spq.PropertyId = STORAGE_PROPERTY_ID.StorageDeviceProperty;
            spq.QueryType = STORAGE_QUERY_TYPE.PropertyStandardQuery;

            return DeviceIoControlHelper.InvokeIoControl<STORAGE_DEVICE_DESCRIPTOR, STORAGE_PROPERTY_QUERY>(Handle, IOControlCode.StorageQueryProperty, spq);
        }

    }
}