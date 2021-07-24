#include "pch.h"
#include "ndtklib.h"
#include "rpc.h"
#include "rpc_misc.h"
#include <stdlib.h>
#include <stdio.h>
#include <ctype.h>
#include <string>
#include "windows.h"

using namespace ndtklib;
using namespace Platform;
using namespace Windows::ApplicationModel;
using namespace Windows::Storage;
using namespace Platform;
using namespace std;

#define RPC_S_OK 0

typedef VOID(*__stdcall _QueryTransientObjectSecurityDescriptor) (ULONG, ULONG, PUCHAR);
typedef RPC_STATUS(*__stdcall _RpcBindingCreateW)(RPC_BINDING_HANDLE_TEMPLATE *Template, RPC_BINDING_HANDLE_SECURITY *Security, RPC_BINDING_HANDLE_OPTIONS_V1 *Options, RPC_BINDING_HANDLE *Binding);
typedef RPC_STATUS(*__stdcall _RpcBindingBind)(PRPC_ASYNC_STATE   pAsync, RPC_BINDING_HANDLE Binding, RPC_IF_HANDLE IfSpec);
typedef CLIENT_CALL_RETURN(*__cdecl _NdrClientCall2)(PMIDL_STUB_DESC pStubDescriptor, PFORMAT_STRING  pFormat, ...);

typedef BOOL(*__stdcall _Ndtk_FileCopy)(PUCHAR, PUCHAR, PUCHAR, PUCHAR, PUCHAR, ULONG);
typedef BOOL(*__stdcall _Ndtk_SystemReboot)(PUCHAR, PUCHAR, PUCHAR);
typedef ULONG(*__stdcall _Ndtk_RegQueryValueExW)(PUCHAR, PUCHAR, PUCHAR, ULONG, PUCHAR, PUCHAR, PUCHAR, PUCHAR, ULONG);
typedef ULONG(*__stdcall _Ndtk_RegSetValueExW)(PUCHAR, PUCHAR, PUCHAR, ULONG, PUCHAR, PUCHAR, ULONG, PUCHAR, ULONG, ULONG);
typedef ULONG(*__stdcall _Ndtk_StopService)(PUCHAR, PUCHAR, PUCHAR, PUCHAR);
typedef ULONG(*__stdcall _Ndtk_StartService)(PUCHAR, PUCHAR, PUCHAR, PUCHAR);


unsigned int mzaddr = 0;
unsigned int cwaddr = 0;
unsigned int mysint = 0;

NRPC::NRPC()
{

}

unsigned int NRPC::Initialize(void)
{
	try {
		// Initialize function pointers

		_QueryTransientObjectSecurityDescriptor QueryTransientObjectSecurityDescriptor = (_QueryTransientObjectSecurityDescriptor)GetProcAddress(L"SECRUNTIME.DLL", "QueryTransientObjectSecurityDescriptor");
		_RpcBindingCreateW RpcBindingCreateW = (_RpcBindingCreateW)GetProcAddress(L"RPCRT4.DLL", "RpcBindingCreateW");
		_RpcBindingBind RpcBindingBind = (_RpcBindingBind)GetProcAddress(L"RPCRT4.DLL", "RpcBindingBind");


		// Query security descriptor
		QueryTransientObjectSecurityDescriptor((ULONG)0x9, (ULONG)rpcInterface, (PUCHAR)rsq.ServerSecurityDescriptor);

		RPC_STATUS rStatus;
		// Create RPC binding
		if (rStatus = RpcBindingCreateW((RPC_BINDING_HANDLE_TEMPLATE*)&bht, (RPC_BINDING_HANDLE_SECURITY*)&bhs, NULL, &rpc_bh))
			return rStatus; //error

		if (rStatus = RpcBindingBind(NULL, rpc_bh, hello_v1_0_c_ifspec))
			return rStatus; //error

		return RPC_S_OK;
	}
	catch (...) {
		return 0x80004005;
	}
}



unsigned int NRPC::FileCopy(String^ src, String^ dst, unsigned int flags)
{
	//////////////////////////////////////////////////////////////////
	//
	// src:   source filename
	// dst:   destination filename
	// flags: COPY_FILE_ALLOW_DECRYPTED_DESTINATION 0x00000008 
	//        COPY_FILE_COPY_SYMLINK 0x00000800
	//        COPY_FILE_FAIL_IF_EXISTS 0x00000001
	//        COPY_FILE_NO_BUFFERING 0x00001000
	//        COPY_FILE_OPEN_SOURCE_FOR_WRITE 0x00000004
	//        COPY_FILE_RESTARTABLE 0x00000002
	// 
	//return code: 0 = success
	//		       >0 = error code
	//
	/////////////////////////////////////////////////////////////////

	try {
		ULONG rStatus;

		_Ndtk_FileCopy Ndtk_CopyFile = (_Ndtk_FileCopy)GetProcAddress(L"RPCRT4.DLL", "NdrClientCall2");

		if (rStatus = Ndtk_CopyFile((PUCHAR)&hello_StubDesc, (PUCHAR)&hello__MIDL_ProcFormatString.Format[168], (PUCHAR)rpc_bh, (PUCHAR)src->Data(), (PUCHAR)dst->Data(), (ULONG)flags))
			return rStatus;

		return 0;
	}
	catch (...) {
		return 0x80004005;
	}
}

unsigned int NRPC::SystemReboot()
{
	try {
		ULONG rStatus;

		_Ndtk_SystemReboot Ndtk_SystemReboot = (_Ndtk_SystemReboot)GetProcAddress(L"RPCRT4.DLL", "NdrClientCall2");

		if (rStatus = Ndtk_SystemReboot((PUCHAR)&hello_StubDesc, (PUCHAR)&hello__MIDL_ProcFormatString.Format[228], (PUCHAR)rpc_bh))
			return rStatus;

		return 0;
	}
	catch (...) {
		return 0x80004005;
	}
}

unsigned int NRPC::StopService(String^ servicename)
{
	//////////////////////////////////////////////////////////////////
	//
	//servicename: self explanatory
	//
	//////////////////////////////////////////////////////////////////

	try {
		ULONG rStatus;

		_Ndtk_StopService Ndtk_StopService = (_Ndtk_StopService)GetProcAddress(L"RPCRT4.DLL", "NdrClientCall2");

		if (rStatus = Ndtk_StopService((PUCHAR)&hello_StubDesc, (PUCHAR)&hello__MIDL_ProcFormatString.Format[744], (PUCHAR)rpc_bh, (PUCHAR)servicename->Data()))
			return rStatus;

		return 0;
	}
	catch (...) {
		return 0x80004005;
	}
}

unsigned int NRPC::StartService(String^ servicename)
{
	//////////////////////////////////////////////////////////////////
	//
	//servicename: self explanatory
	//
	//////////////////////////////////////////////////////////////////

	try {
		ULONG rStatus;

		_Ndtk_StartService Ndtk_StartService = (_Ndtk_StartService)GetProcAddress(L"RPCRT4.DLL", "NdrClientCall2");

		if (rStatus = Ndtk_StartService((PUCHAR)&hello_StubDesc, (PUCHAR)&hello__MIDL_ProcFormatString.Format[698], (PUCHAR)rpc_bh, (PUCHAR)servicename->Data()))
			return rStatus;

		return 0;
	}
	catch (...) {
		return 0x80004005;
	}
}

unsigned int NRPC::RegQueryValue(unsigned int hKey, String^ subkey, String^ value, unsigned int type, Platform::WriteOnlyArray<uint8>^ buffer)
{

	//////////////////////////////////////////////////////////////////
	//
	//hKey: 0 HKEY_CLASSES_ROOT 
	//		1 HKEY_LOCAL_MACHINE
	//		2 HKEY_CURRENT_USER
	//		3 HKEY_CURRENT_CONFIG
	//		4 HKEY_USERS
	//subKey: ex: software\microsoft\mtp
	//value : ex: Datastore
	//type:		1 REG_SZ		
	//			2 REG_EXPAND_SZ
	//			3 REG_BINARY
	//			4 REG_DWORD
	//			5 REG_DWORD_BIG_ENDIAN
	//			6 REG_LINK
	//			7 REG_MULTI_SZ
	//			8 REG_RESOURCE_LIST
	//			9 REG_FULL_RESOURCE_DESCRIPTOR
	//			10 REG_RESOURCE_REQUIREMENTS_LIST
	//			11 REG_QWORD
	//buffer: will get the raw bytes of the registry value. It is the responsibility of the caller to allocate, parse the contents, and free the buffer.
	//
	//return code:  COM error code. E_SUCCESS (0) if success, 0x8007xxxx if fail.
	//              The error code is <the Win32 error code returned by the registry APIs> ORed with 0x80070000.
	//              Therefore, even though NdtkSvc doesn't give you any length, you can therefore figure it out via the returned error code.
	//		       
	//
	/////////////////////////////////////////////////////////////////			   

	try {
		_Ndtk_RegQueryValueExW Ndtk_RegQueryValueExW = (_Ndtk_RegQueryValueExW)GetProcAddress(L"RPCRT4.DLL", "NdrClientCall2");

		//query registry value
		return Ndtk_RegQueryValueExW((PUCHAR)&hello_StubDesc, (PUCHAR)&hello__MIDL_ProcFormatString.Format[0], (PUCHAR)rpc_bh, hKey, (PUCHAR)subkey->Data(), (PUCHAR)value->Data(), (PUCHAR)(&type), (PUCHAR)buffer->Data, (ULONG)buffer->Length);
	}
	catch (...) {
		return 0x80004005;
	}
}

unsigned int NRPC::RegSetValue(unsigned int hKey, String^ subkey, String^ value, unsigned int type, const Platform::Array<uint8>^ buffer)
{

	//////////////////////////////////////////////////////////////////
	//
	//hKey: 0 HKEY_CLASSES_ROOT 
	//		1 HKEY_LOCAL_MACHINE
	//		2 HKEY_CURRENT_USER
	//		3 HKEY_CURRENT_CONFIG
	//		4 HKEY_USERS
	//subKey: ex: software\microsoft\mtp
	//value : ex: Datastore
	//type:		1 REG_SZ		
	//			2 REG_EXPAND_SZ
	//			3 REG_BINARY
	//			4 REG_DWORD
	//			5 REG_DWORD_BIG_ENDIAN
	//			6 REG_LINK
	//			7 REG_MULTI_SZ		
	//			8 REG_RESOURCE_LIST
	//			9 REG_FULL_RESOURCE_DESCRIPTOR
	//			10 REG_RESOURCE_REQUIREMENTS_LIST
	//			11 REG_QWORD
	//buffer:   Array<uint8>^ whose value will be set 
	//
	//return code: 0 = success
	//		       >0 = error code (COM error code, registry API Win32 error ORed with 0x80070000)
	// Read the MSDN docs for RegSetValueExW, we just pass straight to ndtk which passes straight to there!
	//
	/////////////////////////////////////////////////////////////////	

	try {
		ULONG rStatus;
		_Ndtk_RegSetValueExW Ndtk_RegSetValueExW = (_Ndtk_RegSetValueExW)GetProcAddress(L"RPCRT4.DLL", "NdrClientCall2");
		//set registry value
		if (rStatus = Ndtk_RegSetValueExW((PUCHAR)&hello_StubDesc, (PUCHAR)&hello__MIDL_ProcFormatString.Format[80], (PUCHAR)rpc_bh, hKey, (PUCHAR)subkey->Data(), (PUCHAR)value->Data(), (ULONG)(type), (PUCHAR)buffer->Data, (ULONG)buffer->Length, 0x1))
			return rStatus;

		return 0;
	}
	catch (...) {
		return 0x80004005;
	}
}
