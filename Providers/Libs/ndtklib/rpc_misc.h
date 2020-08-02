#ifndef RPC_MISC_H
#define RPC_MISC_H


#include "rpc.h"
#include "rpcndr.h"
#include "stdlib.h"
#include "string.h"


#define TYPE_FORMAT_STRING_SIZE      162                                 
#define PROC_FORMAT_STRING_SIZE      8474                                
#define EXPR_FORMAT_STRING_SIZE      1                                 
#define TRANSMIT_AS_TABLE_SIZE       0            
#define WIRE_MARSHAL_TABLE_SIZE      0            
#define GENERIC_BINDING_TABLE_SIZE   0  
#define RPC_SECURITY_QOS_V5 RPC_SECURITY_QOS_V5_W



typedef struct _hello_MIDL_TYPE_FORMAT_STRING
{
	short          Pad;
	unsigned char  Format[TYPE_FORMAT_STRING_SIZE];
} hello_MIDL_TYPE_FORMAT_STRING;


typedef struct _hello_MIDL_PROC_FORMAT_STRING
{
	short          Pad;
	unsigned char  Format[PROC_FORMAT_STRING_SIZE];
} hello_MIDL_PROC_FORMAT_STRING;


typedef struct _hello_MIDL_EXPR_FORMAT_STRING
{
	long          Pad;
	unsigned char  Format[EXPR_FORMAT_STRING_SIZE];
} hello_MIDL_EXPR_FORMAT_STRING;


typedef struct _RPC_BINDING_HANDLE_TEMPLATE {
	unsigned long  Version;
	unsigned long  Flags;
	unsigned long  ProtocolSequence;
	unsigned short *NetworkAddress;
	unsigned short *StringEndpoint;
	union {
		unsigned short *Reserved;
	} u1;
	UUID           ObjectUuid;
}  RPC_BINDING_HANDLE_TEMPLATE;


typedef struct {
	unsigned long           Version;
	unsigned short          *ServerPrincName;
	unsigned long           AuthnLevel;
	unsigned long           AuthnSvc;
	SEC_WINNT_AUTH_IDENTITY *AuthIdentity;
	RPC_SECURITY_QOS        *SecurityQos;
}  RPC_BINDING_HANDLE_SECURITY;

typedef struct _RPC_BINDING_HANDLE_TEMPLATE_V1_W {
	unsigned long Version;
	unsigned long Flags;
	unsigned long ProtocolSequence;
	unsigned short *NetworkAddress;
	unsigned short *StringEndpoint;
	union
	{
		unsigned short *Reserved;
	} u1;
	UUID ObjectUuid;
} RPC_BINDING_HANDLE_TEMPLATE_V1_W, *PRPC_BINDING_HANDLE_TEMPLATE_V1_W;



typedef struct _RPC_BINDING_HANDLE_SECURITY_V1_W {
	unsigned long Version;
	unsigned short *ServerPrincName;
	unsigned long AuthnLevel;
	unsigned long AuthnSvc;
	SEC_WINNT_AUTH_IDENTITY_W *AuthIdentity;
	RPC_SECURITY_QOS *SecurityQos;
} RPC_BINDING_HANDLE_SECURITY_V1_W, *PRPC_BINDING_HANDLE_SECURITY_V1_W;

typedef struct _STARTUPINFO {
	DWORD  cb;
	LPTSTR lpReserved;
	LPTSTR lpDesktop;
	LPTSTR lpTitle;
	DWORD  dwX;
	DWORD  dwY;
	DWORD  dwXSize;
	DWORD  dwYSize;
	DWORD  dwXCountChars;
	DWORD  dwYCountChars;
	DWORD  dwFillAttribute;
	DWORD  dwFlags;
	WORD   wShowWindow;
	WORD   cbReserved2;
	LPBYTE lpReserved2;
	HANDLE hStdInput;
	HANDLE hStdOutput;
	HANDLE hStdError;
} STARTUPINFO, *LPSTARTUPINFO;

typedef struct _PROCESS_INFORMATION {
	HANDLE hProcess;
	HANDLE hThread;
	DWORD  dwProcessId;
	DWORD  dwThreadId;
} PROCESS_INFORMATION, *LPPROCESS_INFORMATION;




typedef enum _RPC_ASYNC_EVENT {
	RpcCallComplete,
	RpcSendComplete,
	RpcReceiveComplete,
	RpcClientDisconnect,
	RpcClientCancel
} RPC_ASYNC_EVENT;


typedef struct _RPC_ASYNC_STATE {
	unsigned int                Size;
	unsigned long               Signature;
	long                        Lock;
	unsigned long               Flags;
	void                        *StubInfo;
	void                        *UserInfo;
	void                        *RuntimeInfo;
	RPC_ASYNC_EVENT             Event;
	void      *NotificationType;
	void *u;
	LONG_PTR                    Reserved[4];
} RPC_ASYNC_STATE, *PRPC_ASYNC_STATE;

typedef struct _RPC_BINDING_HANDLE_OPTIONS_V1 {
	unsigned long Version;
	unsigned long Flags;
	unsigned long ComTimeout;
	unsigned long CallTimeout;
} RPC_BINDING_HANDLE_OPTIONS_V1, RPC_BINDING_HANDLE_OPTIONS;

extern handle_t hello_IfHandle;
extern const hello_MIDL_TYPE_FORMAT_STRING hello__MIDL_TypeFormatString;
extern const hello_MIDL_PROC_FORMAT_STRING hello__MIDL_ProcFormatString;
extern const hello_MIDL_EXPR_FORMAT_STRING hello__MIDL_ExprFormatString;
extern const MIDL_STUB_DESC hello_StubDesc;
extern RPC_IF_HANDLE hello_v1_0_c_ifspec;
extern RPC_IF_HANDLE hello_v1_0_s_ifspec;
extern RPC_BINDING_HANDLE rpc_bh;
extern RPC_BINDING_HANDLE_TEMPLATE bht;
extern RPC_SECURITY_QOS_V5 rsq;
extern RPC_BINDING_HANDLE_SECURITY bhs;
extern wchar_t* rpcInterface;


void __RPC_FAR * __RPC_USER midl_user_allocate(size_t cBytes);
void __RPC_USER midl_user_free(void __RPC_FAR * p);

#endif // !RPC_MISC_H