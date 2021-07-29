#ifndef NDTKLIB_H
#define NDTKLIB_H

using namespace Platform;

namespace ndtklib
{
    public ref class NRPC sealed
    {
    public:
        NRPC();
		unsigned int NRPC::Initialize(void);
		unsigned int NRPC::FileCopy(String^ src, String^ dst, unsigned int flags);
		unsigned int NRPC::StopService(String^ servicename);
		unsigned int NRPC::StartService(String^ servicename);
		unsigned int NRPC::RegQueryValue(unsigned int hKey, String^ subkey, String^ value, unsigned int type, Platform::WriteOnlyArray<uint8>^ buffer);
		unsigned int NRPC::RegSetValue(unsigned int hKey, String^ subkey, String^ value, unsigned int type, const Platform::Array<uint8>^ buffer);
		unsigned int NRPC::SystemReboot();
    };
}

#endif