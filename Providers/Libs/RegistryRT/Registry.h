#pragma once
#include <collection.h>
#include <windows.h>

namespace RegistryRT
{

	public enum class RegistryType
	{
		None = REG_NONE,
		String = REG_SZ,
		VariableString = REG_EXPAND_SZ,
		Binary = REG_BINARY,
		Integer = REG_DWORD,
		IntegerBigEndian = REG_DWORD_BIG_ENDIAN,
		SymbolicLink = REG_LINK,
		MultiString = REG_MULTI_SZ,
		ResourceList = REG_RESOURCE_LIST,
		HardwareResourceList = REG_FULL_RESOURCE_DESCRIPTOR,
		ResourceRequirement = REG_RESOURCE_REQUIREMENTS_LIST,
		Long = REG_QWORD
	};

	#undef HKEY_CLASSES_ROOT
	#undef HKEY_CURRENT_USER
	#undef HKEY_LOCAL_MACHINE
	#undef HKEY_USERS
	#undef HKEY_CURRENT_CONFIG
	#undef HKEY_CURRENT_USER_LOCAL_SETTINGS

	public enum class RegistryHive
	{
		HKEY_CLASSES_ROOT,
		HKEY_CURRENT_USER,
		HKEY_LOCAL_MACHINE,
		HKEY_USERS,
		HKEY_CURRENT_CONFIG,
		HKEY_CURRENT_USER_LOCAL_SETTINGS
	};


	typedef DWORD ULONG;
	typedef WORD  USHORT, *USHORT_PTR;
	typedef ULONG NTSTATUS, *PNTSTATUS;

	typedef ULONG ACCESS_MASK, *PACCESS_MASK;


	// this is taken from "ntddk.h" header file
	//
	// Define the create disposition values
	//
	#define FILE_SUPERSEDE					0x00000000
	#define FILE_OPEN						0x00000001
	#define FILE_CREATE						0x00000002
	#define FILE_OPEN_IF					0x00000003
	#define FILE_OVERWRITE					0x00000004
	#define FILE_OVERWRITE_IF				0x00000005
	#define FILE_MAXIMUM_DISPOSITION		0x00000005

	//  Function failed during execution.
	//
	#define ERROR_FUNCTION_FAILED			1627L

	//
	// Define the create/open option flags
	//
	#define FILE_DIRECTORY_FILE				0x00000001
	#define FILE_WRITE_THROUGH				0x00000002
	#define FILE_SEQUENTIAL_ONLY			0x00000004
	#define FILE_NO_INTERMEDIATE_BUFFERING	0x00000008

	#define FILE_SYNCHRONOUS_IO_ALERT		0x00000010
	#define FILE_SYNCHRONOUS_IO_NONALERT	0x00000020
	#define FILE_NON_DIRECTORY_FILE			0x00000040
	#define FILE_CREATE_TREE_CONNECTION		0x00000080

	#define FILE_COMPLETE_IF_OPLOCKED		0x00000100
	#define FILE_NO_EA_KNOWLEDGE			0x00000200
	#define FILE_OPEN_FOR_RECOVERY			0x00000400
	#define FILE_RANDOM_ACCESS				0x00000800

	#define FILE_DELETE_ON_CLOSE			0x00001000
	#define FILE_OPEN_BY_FILE_ID			0x00002000
	#define FILE_OPEN_FOR_BACKUP_INTENT		0x00004000
	#define FILE_NO_COMPRESSION				0x00008000

	#define FILE_RESERVE_OPFILTER			0x00100000
	#define FILE_OPEN_REPARSE_POINT			0x00200000
	#define FILE_OPEN_NO_RECALL				0x00400000
	#define FILE_OPEN_FOR_FREE_SPACE_QUERY	0x00800000

	#define FILE_COPY_STRUCTURED_STORAGE	0x00000041
	#define FILE_STRUCTURED_STORAGE			0x00000441

	#define FILE_VALID_OPTION_FLAGS			0x00ffffff
	#define FILE_VALID_PIPE_OPTION_FLAGS	0x00000032
	#define FILE_VALID_MAILSLOT_OPTION_FLAGS	0x00000032
	#define FILE_VALID_SET_FLAGS			0x00000036

	// this is taken from "NTStatus.h" header file
	#define STATUS_SUCCESS				((NTSTATUS)0x00000000L) // ntsubauth
	#define STATUS_BUFFER_OVERFLOW		((NTSTATUS)0x80000005L)
	#define STATUS_INVALID_PARAMETER	((NTSTATUS)0xC000000DL)
	#define STATUS_ACCESS_DENIED		((NTSTATUS)0xC0000022L)
	#define STATUS_NO_MORE_ENTRIES		((NTSTATUS)0x8000001AL)
	#define STATUS_OBJECT_TYPE_MISMATCH ((NTSTATUS)0xC0000024L)

	// 
	// I hate to type ;-)
	#define NT_SUCCESS(Status) ((NTSTATUS)(Status) == STATUS_SUCCESS)
	#define HKU		HKEY_USERS
	#define HKLM	HKEY_LOCAL_MACHINE
	#define HKCU	HKEY_CURRENT_USER
	#define HKCC	HKEY_CURRENT_CONFIG
	#define HKCR	HKEY_CLASSES_ROOT

	typedef struct _OBJECT_BASIC_INFORMATION {
		ULONG Attributes;
		ACCESS_MASK GrantedAccess;
		ULONG HandleCount;
		ULONG PointerCount;
		ULONG PagedPoolUsage;
		ULONG NonPagedPoolUsage;
		ULONG Reserved[3];
		ULONG NameInformationLength;
		ULONG TypeInformationLength;
		ULONG SecurityDescriptorLength;
		LARGE_INTEGER CreateTime;
	} OBJECT_BASIC_INFORMATION, *POBJECT_BASIC_INFORMATION;

	typedef struct _UNICODE_STRING
	{
		USHORT Length;
		USHORT MaximumLength;
		PWSTR  Buffer;
	} UNICODE_STRING;
	typedef UNICODE_STRING *PUNICODE_STRING;

	// InitializeUnicodeStrings (WCHAR wstr, BOOL hidden, UNICODE_STRING* us);
	#define InitializeUnicodeStrings( wstr, hidden, us ) {                                \
		(us)->Buffer = (PWSTR)wstr;                                                              \
		(us)->Length = (USHORT)(wcslen((const wchar_t *)wstr) * sizeof(WCHAR)) + (sizeof(WCHAR) * hidden); \
		(us)->MaximumLength = (us)->Length+4;                                             \
	}

	typedef struct _STRING
	{
		USHORT Length;
		USHORT MaximumLength;
		//#ifdef MIDL_PASS
		//    [size_is(MaximumLength), length_is(Length) ]
		//#endif // MIDL_PASS
		PCHAR Buffer;
	} STRING;
	typedef STRING *PSTRING;
	typedef STRING OEM_STRING;
	typedef STRING *POEM_STRING;
	typedef STRING ANSI_STRING;
	typedef STRING *PANSI_STRING;

	// =================================================================
	// Key query structures
	// =================================================================
	typedef struct _KEY_BASIC_INFORMATION
	{
		LARGE_INTEGER LastWriteTime;// The last time the key or any of its values changed.
		ULONG TitleIndex;			// Device and intermediate drivers should ignore this member.
		ULONG NameLength;			// The size in bytes of the following name, including the zero-terminating character.
		WCHAR Name[1];				// A zero-terminated Unicode string naming the key.
	} KEY_BASIC_INFORMATION;
	typedef KEY_BASIC_INFORMATION *PKEY_BASIC_INFORMATION;

	typedef struct _KEY_FULL_INFORMATION
	{
		LARGE_INTEGER LastWriteTime;// The last time the key or any of its values changed.
		ULONG TitleIndex;			// Device and intermediate drivers should ignore this member.
		ULONG ClassOffset;			// The offset from the start of this structure to the Class member.
		ULONG ClassLength;			// The number of bytes in the Class name.
		ULONG SubKeys;				// The number of subkeys for the key.
		ULONG MaxNameLen;			// The maximum length of any name for a subkey.
		ULONG MaxClassLen;			// The maximum length for a Class name.
		ULONG Values;				// The number of value entries.
		ULONG MaxValueNameLen;		// The maximum length of any value entry name.
		ULONG MaxValueDataLen;		// The maximum length of any value entry data field.
		WCHAR Class[1];				// A zero-terminated Unicode string naming the class of the key.
	} KEY_FULL_INFORMATION;
	typedef KEY_FULL_INFORMATION *PKEY_FULL_INFORMATION;

	typedef struct _KEY_NODE_INFORMATION
	{
		LARGE_INTEGER LastWriteTime;// The last time the key or any of its values changed.
		ULONG TitleIndex;			// Device and intermediate drivers should ignore this member.
		ULONG ClassOffset;			// The offset from the start of this structure to the Class member.
		ULONG ClassLength;			// The number of bytes in the Class name.
		ULONG NameLength;			// The size in bytes of the following name, including the zero-terminating character.
		WCHAR Name[1];				// A zero-terminated Unicode string naming the key.
	} KEY_NODE_INFORMATION;
	typedef KEY_NODE_INFORMATION *PKEY_NODE_INFORMATION;

	// end_wdm
	typedef struct _KEY_NAME_INFORMATION
	{
		ULONG   NameLength;
		WCHAR   Name[1];            // Variable length string
	} KEY_NAME_INFORMATION, *PKEY_NAME_INFORMATION;
	typedef KEY_NAME_INFORMATION *PKEY_NAME_INFORMATION;

	// begin_wdm
	typedef enum _KEY_INFORMATION_CLASS
	{
		KeyBasicInformation,
		KeyNodeInformation,
		KeyFullInformation
		// end_wdm
		,
		KeyNameInformation
		// begin_wdm
	} KEY_INFORMATION_CLASS;

	typedef struct _KEY_WRITE_TIME_INFORMATION
	{
		LARGE_INTEGER LastWriteTime;
	} KEY_WRITE_TIME_INFORMATION;
	typedef KEY_WRITE_TIME_INFORMATION *PKEY_WRITE_TIME_INFORMATION;

	typedef enum _KEY_SET_INFORMATION_CLASS
	{
		KeyWriteTimeInformation
	} KEY_SET_INFORMATION_CLASS;


	// =================================================================
	// DesiredAccess Flags
	// =================================================================
	// KEY_QUERY_VALUE			Value entries for the key can be read. 
	// KEY_SET_VALUE			Value entries for the key can be written. 
	// KEY_CREATE_SUB_KEY		Subkeys for the key can be created. 
	// KEY_ENUMERATE_SUB_KEYS	All subkeys for the key can be read. 
	// KEY_NOTIFY				This flag is irrelevant to device and intermediate drivers, 
	//							and to other kernel-mode code. 
	// KEY_CREATE_LINK			A symbolic link to the key can be created. This flag is 
	//							irrelvant to device and intermediate drivers. 
	// 
	// KEY_QUERY_VALUE			(0x0001)
	// KEY_SET_VALUE			(0x0002)
	// KEY_CREATE_SUB_KEY		(0x0004)
	// KEY_ENUMERATE_SUB_KEYS	(0x0008)
	// KEY_NOTIFY				(0x0010)
	// KEY_CREATE_LINK			(0x0020)
	//
	//
	// =================================================================
	// DesiredAccess to Key Values
	// =================================================================
	// KEY_READ				STANDARD_RIGHTS_READ, KEY_QUERY_VALUE, 
	//						KEY_ENUMERATE_SUB_KEYS, and KEY_NOTIFY 
	// KEY_WRITE			STANDARD_RIGHTS_WRITE, KEY_SET_VALUE, and KEY_CREATE_SUBKEY 
	// KEY_EXECUTE			KEY_READ. This value is irrelevant to device and intermediate 
	// 						drivers. 
	// KEY_ALL_ACCESS		STANDARD_RIGHTS_ALL, KEY_QUERY_VALUE, KEY_SET_VALUE, 
	// 						KEY_CREATE_SUB_KEY, KEY_ENUMERATE_SUBKEY, KEY_NOTIFY 
	// 						and KEY_CREATE_LINK 
	// 
	// 
	// =================================================================
	// CreateOptions Values
	// =================================================================
	// REG_OPTION_NON_VOLATILE		Key is preserved when the system is rebooted. 
	// REG_OPTION_VOLATILE			Key is not to be stored across boots. 
	// REG_OPTION_CREATE_LINK		The created key is a symbolic link. This value is 
	//								irrelevant to device and intermediate drivers. 
	// REG_OPTION_BACKUP_RESTORE	Key is being opened or created with special privileges 
	//								allowing backup/restore operations. This value is 
	// 								irrelevant to device and intermediate drivers. 
	// 
	// REG_OPTION_NON_VOLATILE		(0x00000000L)
	// REG_OPTION_VOLATILE			(0x00000001L)
	// REG_OPTION_CREATE_LINK		(0x00000002L)
	// REG_OPTION_BACKUP_RESTORE	(0x00000004L)
	// 
	// 
	// =================================================================
	// Disposition Values
	// =================================================================
	// REG_CREATED_NEW_KEY		A new key object was created. 
	// REG_OPENED_EXISTING_KEY	An existing key object was opened. 
	// 
	// REG_CREATED_NEW_KEY		(0x00000001L)
	// REG_OPENED_EXISTING_KEY	(0x00000002L)
	//
	//
	// =================================================================
	// Value entry query structures
	// REG_XXX Type Value:
	// =================================================================
	// REG_BINARY			Binary data in any form 
	// REG_DWORD			A 4-byte numerical value (32-bit number) 
	// REG_DWORD_LITTLE_ENDIAN  A 4-byte numerical value whose least significant 
	//						byte is at the lowest address 
	// REG_QWORD			64-bit number. 
	// REG_QWORD_LITTLE_ENDIAN	A 64-bit number in little-endian format. This is 
	//						equivalent to REG_QWORD. 
	// REG_DWORD_BIG_ENDIAN A 4-byte numerical value whose least significant byte 
	//						is at the highest address 
	// REG_EXPAND_SZ		A zero-terminated Unicode string, containing unexpanded 
	//						references to environment variables, such as "%PATH%" 
	// REG_LINK				A Unicode string naming a symbolic link. This type is 
	//						irrelevant to device and intermediate drivers 
	// REG_MULTI_SZ			An array of zero-terminated strings, terminated by another zero 
	// REG_NONE				Data with no particular type 
	// REG_SZ				A zero-terminated Unicode string 
	// REG_RESOURCE_LIST	A device driver's list of hardware resources, used by the driver 
	//						or one of the physical devices it controls, in the \ResourceMap tree 
	// REG_RESOURCE_REQUIREMENTS_LIST	A device driver's list of possible hardware resources 
	//						it or one of the physical devices it controls can use, from which 
	//						the system writes a subset into the \ResourceMap tree 
	// REG_FULL_RESOURCE_DESCRIPTOR		A list of hardware resources that a physical device 
	//						is using, detected and written into the \HardwareDescription tree 
	//						by the system 
	//
	// =================================================================
	typedef struct _KEY_VALUE_BASIC_INFORMATION
	{
		ULONG TitleIndex;	// Device and intermediate drivers should ignore this member.
		ULONG Type;			// The system-defined type for the registry value in the 
							// Data member (see the values above).
		ULONG NameLength;	// The size in bytes of the following value entry name, 
							// including the zero-terminating character.
		WCHAR Name[1];		// A zero-terminated Unicode string naming a value entry of 
							// the key.
	} KEY_VALUE_BASIC_INFORMATION;
	typedef KEY_VALUE_BASIC_INFORMATION *PKEY_VALUE_BASIC_INFORMATION;

	typedef struct _KEY_VALUE_FULL_INFORMATION
	{
		ULONG TitleIndex;	// Device and intermediate drivers should ignore this member.
		ULONG Type;			// The system-defined type for the registry value in the 
							// Data member (see the values above).
		ULONG DataOffset;	// The offset from the start of this structure to the data 
							// immediately following the Name string.
		ULONG DataLength;	// The number of bytes of registry information for the value 
							// entry identified by Name.
		ULONG NameLength;	// The size in bytes of the following value entry name, 
							// including the zero-terminating character.
		WCHAR Name[1];		// A zero-terminated Unicode string naming a value entry of 
							// the key.
							//	WCHAR Data[1];      // Variable size data not declared
	} KEY_VALUE_FULL_INFORMATION;
	typedef KEY_VALUE_FULL_INFORMATION *PKEY_VALUE_FULL_INFORMATION;

	typedef struct _KEY_VALUE_PARTIAL_INFORMATION
	{
		ULONG TitleIndex;	// Device and intermediate drivers should ignore this member.
		ULONG Type;			// The system-defined type for the registry value in the 
							// Data member (see the values above).
		ULONG DataLength;	// The size in bytes of the Data member.
		UCHAR Data[1];		// A value entry of the key.
	} KEY_VALUE_PARTIAL_INFORMATION;
	typedef KEY_VALUE_PARTIAL_INFORMATION *PKEY_VALUE_PARTIAL_INFORMATION;

	typedef struct _KEY_VALUE_ENTRY
	{
		PUNICODE_STRING ValueName;
		ULONG           DataLength;
		ULONG           DataOffset;
		ULONG           Type;
	} KEY_VALUE_ENTRY;
	typedef KEY_VALUE_ENTRY *PKEY_VALUE_ENTRY;

	typedef enum _KEY_VALUE_INFORMATION_CLASS
	{
		KeyValueBasicInformation,
		KeyValueFullInformation,
		KeyValuePartialInformation,
	} KEY_VALUE_INFORMATION_CLASS;

	typedef struct _KEY_MULTIPLE_VALUE_INFORMATION
	{
		PUNICODE_STRING	ValueName;
		ULONG			DataLength;
		ULONG			DataOffset;
		ULONG			Type;
	} KEY_MULTIPLE_VALUE_INFORMATION;
	typedef KEY_MULTIPLE_VALUE_INFORMATION *PKEY_MULTIPLE_VALUE_INFORMATION;

	typedef struct _IO_STATUS_BLOCK
	{
		union
		{
			NTSTATUS	Status;
			PVOID		Pointer;
		};
		ULONG_PTR	Information;
	} IO_STATUS_BLOCK;
	typedef IO_STATUS_BLOCK *PIO_STATUS_BLOCK;

	typedef void (NTAPI *PIO_APC_ROUTINE)
		(
			IN PVOID ApcContext,
			IN PIO_STATUS_BLOCK IoStatusBlock,
			IN ULONG Reserved
			);


	//
	// ClientId
	//
	typedef struct _CLIENT_ID
	{
		HANDLE UniqueProcess;
		HANDLE UniqueThread;
	} CLIENT_ID;
	typedef CLIENT_ID *PCLIENT_ID;


	// =================================================================
	//
	// Valid values for the Attributes field
	//
	// This handle can be inherited by child processes of the current process.
	#define OBJ_INHERIT				0x00000002L

	// This flag only applies to objects that are named within the Object Manager. 
	// By default, such objects are deleted when all open handles to them are closed. 
	// If this flag is specified, the object is not deleted when all open handles are 
	// closed. Drivers can use ZwMakeTemporaryObject to delete permanent objects.
	#define OBJ_PERMANENT			0x00000010L

	// Only a single handle can be open for this object.
	#define OBJ_EXCLUSIVE			0x00000020L

	// If this flag is specified, a case-insensitive comparison is used when 
	// matching the ObjectName parameter against the names of existing objects. 
	// Otherwise, object names are compared using the default system settings.
	#define OBJ_CASE_INSENSITIVE	0x00000040L

	// If this flag is specified to a routine that creates objects, and that object 
	// already exists then the routine should open that object. Otherwise, the routine 
	// creating the object returns an NTSTATUS code of STATUS_OBJECT_NAME_COLLISION.
	#define OBJ_OPENIF				0x00000080L

	// Specifies that the handle can only be accessed in kernel mode.
	#define OBJ_KERNEL_HANDLE		0x00000200L

	// The routine opening the handle should enforce all access checks 
	// for the object, even if the handle is being opened in kernel mode.
	#define OBJ_FORCE_ACCESS_CHECK	0x00000400L

	//
	#define OBJ_VALID_ATTRIBUTES    0x000007F2L


	typedef struct _OBJECT_ATTRIBUTES {
		ULONG Length;
		HANDLE RootDirectory;
		PUNICODE_STRING ObjectName;
		ULONG Attributes;
		PVOID SecurityDescriptor;        // Points to type SECURITY_DESCRIPTOR
		PVOID SecurityQualityOfService;  // Points to type SECURITY_QUALITY_OF_SERVICE
	} OBJECT_ATTRIBUTES;
	typedef OBJECT_ATTRIBUTES *POBJECT_ATTRIBUTES;

	#define InitializeObjectAttributes( p, n, a, r, s ) { \
		(p)->Length = sizeof( OBJECT_ATTRIBUTES );        \
		(p)->RootDirectory = r;                           \
		(p)->Attributes = a;                              \
		(p)->ObjectName = n;                              \
		(p)->SecurityDescriptor = s;                      \
		(p)->SecurityQualityOfService = NULL;             \
		}
	//
	// =================================================================


	#define RtlFillMemory(Destination,Length,Fill) memset((Destination),(Fill),(Length))
	#define RtlZeroMemory(Destination,Length) memset((Destination),0,(Length))
	#define RtlCopyMemory(Destination,Source,Length) memcpy((Destination),(Source),(Length))
	#define RtlMoveMemory(Destination,Source,Length) memmove((Destination),(Source),(Length))


	// =================================================================
	//  NTDLL Entry Points
	// =================================================================
	/*
	Mapping Native APIs to Win32 Registry functions

	// Creates or opens a Registry key.
	NtCreateKey				RegCreateKey

	// Opens an existing Registry key.
	NtOpenKey				RegOpenKey

	// Deletes a Registry key.
	NtDeleteKey				RegDeleteKey

	// Deletes a value.
	NtDeleteValueKey		RegDeleteValue

	// Enumerates the subkeys of a key.
	NtEnumerateKey			RegEnumKey, RegEnumKeyEx

	// Enumerates the values within a key.
	NtEnumerateValueKey		RegEnumValue

	// Flushes changes back to the Registry on disk.
	NtFlushKey				RegFlushKey

	// Gets the Registry rolling. The single parameter to this
	// specifies whether its a setup boot or a normal boot.
	NtInitializeRegistry	NONE

	// Allows a program to be notified of changes to a particular
	// key or its subkeys.
	NtNotifyChangeKey		RegNotifyChangeKeyValue

	// Queries information about a key.
	NtQueryKey				RegQueryKey

	// Retrieves information about multiple specified values.
	// This API was introduced in NT 4.0.
	NtQueryMultiplValueKey	RegQueryMultipleValues

	// Retrieves information about a specified value.
	NtQueryValueKey			RegQueryValue, RegQueryValueEx

	// Changes the backing file for a key and its subkeys.
	// Used for backup/restore.
	NtReplaceKey			RegReplaceKey

	// Saves the contents of a key and subkey to a file.
	NtSaveKey				RegSaveKey

	// Loads the contents of a key from a specified file.
	NtRestoreKey			RegRestoreKey

	// Sets attributes of a key.
	NtSetInformationKey		NONE

	// Sets the data associated with a value.
	NtSetValueKey			RegSetValue, RegSetValueEx

	// Loads a hive file into the Registry.
	NtLoadKey				RegLoadKey

	// Introduced in NT 4.0. Allows for options on loading a hive.
	NtLoadKey2				NONE

	// Unloads a hive from the Registry.
	NtUnloadKey				RegUnloadKey

	// New to WinXP. Makes key storage adjacent.
	NtCompactKeys			NONE

	// New to WinXP. Performs in-place compaction of a hive.
	NtCompressKey			NONE

	// New to WinXP. Locks a registry key for modification.
	NtLockRegistryKey		NONE

	// New to WinXP. Renames a Registry key.
	NtRenameKey				NONE
	NtRenameKey(IN HANDLE KeyHandle, IN PUNICODE_STRING ReplacementName);

	// New to WinXP. Saves the contents of a key and its subkeys to a file.
	NtSaveKeyEx				RegSaveKeyEx

	// New to WinXP. Unloads a hive from the Registry.
	NtUnloadKeyEx			NONE

	// New to Server 2K3. Loads a hive into the Registry.
	NtLoadKeyEx				NONE

	// New to Serer 2K3. Unloads a hive from the Registry.
	NtUnloadKey2			NONE

	// New to Server 2003. Returns the keys opened beneath a specified key.
	NtQueryOpenSubKeysEx	NONE

	*/


	// =================================================================
	//  RTL String Functions
	// =================================================================

	// RtlInitString
	typedef NTSTATUS(STDAPICALLTYPE RTLINITSTRING)
		(
			IN OUT PSTRING DestinationString,
			IN LPCSTR SourceString
			);
	//IN PCSZ
	typedef RTLINITSTRING FAR * LPRTLINITSTRING;

	// RtlInitAnsiString
	typedef NTSTATUS(STDAPICALLTYPE RTLINITANSISTRING)
		(
			IN OUT PANSI_STRING DestinationString,
			IN LPCSTR SourceString
			);
	typedef RTLINITANSISTRING FAR * LPRTLINITANSISTRING;

	// RtlInitUnicodeString
	typedef NTSTATUS(STDAPICALLTYPE RTLINITUNICODESTRING)
		(
			IN OUT PUNICODE_STRING DestinationString,
			IN LPCWSTR SourceString
			);
	typedef RTLINITUNICODESTRING FAR * LPRTLINITUNICODESTRING;

	// RtlAnsiStringToUnicodeString
	typedef NTSTATUS(STDAPICALLTYPE RTLANSISTRINGTOUNICODESTRING)
		(
			IN OUT PUNICODE_STRING	DestinationString,
			IN PANSI_STRING			SourceString,
			IN BOOLEAN				AllocateDestinationString
			);
	typedef RTLANSISTRINGTOUNICODESTRING FAR * LPRTLANSISTRINGTOUNICODESTRING;

	// RtlUnicodeStringToAnsiString
	typedef NTSTATUS(STDAPICALLTYPE RTLUNICODESTRINGTOANSISTRING)
		(
			IN OUT PANSI_STRING		DestinationString,
			IN PUNICODE_STRING		SourceString,
			IN BOOLEAN				AllocateDestinationString
			);
	typedef RTLUNICODESTRINGTOANSISTRING FAR * LPRTLUNICODESTRINGTOANSISTRING;

	// RtlFreeString
	typedef NTSTATUS(STDAPICALLTYPE RTLFREESTRING)
		(
			IN PSTRING String
			);
	typedef RTLFREESTRING FAR * LPRTLFREESTRING;

	// RtlFreeAnsiString
	typedef NTSTATUS(STDAPICALLTYPE RTLFREEANSISTRING)
		(
			IN PANSI_STRING AnsiString
			);
	typedef RTLFREEANSISTRING FAR * LPRTLFREEANSISTRING;

	// RtlFreeUnicodeString
	typedef NTSTATUS(STDAPICALLTYPE RTLFREEUNICODESTRING)
		(
			IN PUNICODE_STRING UnicodeString
			);
	typedef RTLFREEUNICODESTRING FAR * LPRTLFREEUNICODESTRING;


	// RtlConvertSidToUnicodeString
	typedef NTSTATUS(STDAPICALLTYPE RTLCONVERTSIDTOUNICODESTRING)(
		OUT PUNICODE_STRING UnicodeString,
		IN  PSID            Sid,
		IN  BOOLEAN         AllocateDestinationString
		);
	typedef RTLCONVERTSIDTOUNICODESTRING FAR * LPRTLCONVERTSIDTOUNICODESTRING;


	//DWORD WINAPI RtlEqualUnicodeString(PUNICODE_STRING s1,PUNICODE_STRING s2,DWORD x);
	//DWORD WINAPI RtlUpcaseUnicodeString(PUNICODE_STRING dest,PUNICODE_STRING src,BOOLEAN doalloc);
	//NTSTATUS WINAPI RtlCompareUnicodeString(PUNICODE_STRING String1, PUNICODE_STRING String2, BOOLEAN CaseInSensitive);

	// =================================================================
	//  END - RTL String Functions
	// =================================================================

	typedef BOOL(WINAPI GETTOKENINFORMATION)
		(
			_In_      HANDLE                  TokenHandle,
			_In_      TOKEN_INFORMATION_CLASS TokenInformationClass,
			_Out_opt_ LPVOID                  TokenInformation,
			_In_      DWORD                   TokenInformationLength,
			_Out_     PDWORD                  ReturnLength
			);
	typedef GETTOKENINFORMATION FAR * LPGETTOKENINFORMATION;

	typedef BOOL(WINAPI OPENPROCESSTOKEN)
		(
			_In_  HANDLE  ProcessHandle,
			_In_  DWORD   DesiredAccess,
			_Out_ PHANDLE TokenHandle
			);
	typedef OPENPROCESSTOKEN FAR * LPOPENPROCESSTOKEN;

	typedef BOOL(WINAPI OPENTHREADTOKEN)
		(
			_In_  HANDLE  ThreadHandle,
			_In_  DWORD   DesiredAccess,
			_In_  BOOL    OpenAsSelf,
			_Out_ PHANDLE TokenHandle
			);
	typedef OPENTHREADTOKEN FAR * LPOPENTHREADTOKEN;

	// NtCreateKey
	typedef NTSTATUS(STDAPICALLTYPE NTCREATEKEY)
		(
			IN HANDLE				KeyHandle,
			IN ULONG				DesiredAccess,
			IN POBJECT_ATTRIBUTES	ObjectAttributes,
			IN ULONG				TitleIndex,
			IN PUNICODE_STRING		Class,			/* optional*/
			IN ULONG				CreateOptions,
			OUT PULONG				Disposition		/* optional*/
			);
	typedef NTCREATEKEY FAR * LPNTCREATEKEY;


	// NtOpenKey
	typedef NTSTATUS(STDAPICALLTYPE NTOPENKEY)
		(
			IN HANDLE				KeyHandle,
			IN ULONG				DesiredAccess,
			IN POBJECT_ATTRIBUTES	ObjectAttributes
			);
	typedef NTOPENKEY FAR * LPNTOPENKEY;

	// NtFlushKey
	typedef NTSTATUS(STDAPICALLTYPE NTFLUSHKEY)
		(
			IN HANDLE KeyHandle
			);
	typedef NTFLUSHKEY FAR * LPNTFLUSHKEY;

	// NtDeleteKey
	typedef NTSTATUS(STDAPICALLTYPE NTDELETEKEY)
		(
			IN HANDLE KeyHandle
			);
	typedef NTDELETEKEY FAR * LPNTDELETEKEY;

	// NtSetValueKey
	typedef NTSTATUS(STDAPICALLTYPE NTSETVALUEKEY)
		(
			IN HANDLE			KeyHandle,
			IN PUNICODE_STRING	ValueName,
			IN ULONG			TitleIndex,			/* optional */
			IN ULONG			Type,
			IN PVOID			Data,
			IN ULONG			DataSize
			);
	typedef NTSETVALUEKEY FAR * LPNTSETVALUEKEY;

	// NtQueryValueKey
	typedef NTSTATUS(STDAPICALLTYPE NTQUERYVALUEKEY)
		(
			// Is the handle, returned by a successful 
			// call to NtCreateKey or NtOpenKey, of key 
			// for which value entries are to be read.
			IN HANDLE			KeyHandle,
			IN PUNICODE_STRING	ValueName,
			IN KEY_VALUE_INFORMATION_CLASS KeyValueInformationClass,
			OUT PVOID			KeyValueInformation,
			IN ULONG			Length,
			OUT PULONG			ResultLength
			);
	typedef NTQUERYVALUEKEY FAR * LPNTQUERYVALUEKEY;


	// NtSetInformationKey
	typedef NTSTATUS(STDAPICALLTYPE NTSETINFORMATIONKEY)
		(
			IN HANDLE	KeyHandle,
			IN KEY_SET_INFORMATION_CLASS KeyInformationClass,
			IN PVOID	KeyInformation,
			IN ULONG	KeyInformationLength
			);
	typedef NTSETINFORMATIONKEY FAR * LPNTSETINFORMATIONKEY;

	// NtQueryKey
	typedef NTSTATUS(STDAPICALLTYPE NTQUERYKEY)
		(
			IN HANDLE	KeyHandle,
			IN KEY_INFORMATION_CLASS KeyInformationClass,
			OUT PVOID	KeyInformation,
			IN ULONG	KeyInformationLength,
			OUT PULONG	ResultLength
			);
	typedef NTQUERYKEY FAR * LPNTQUERYKEY;

	// NtEnumerateKey
	typedef NTSTATUS(STDAPICALLTYPE NTENUMERATEKEY)
		(
			IN HANDLE	KeyHandle,
			IN ULONG	Index,
			IN KEY_INFORMATION_CLASS KeyInformationClass,
			OUT PVOID	KeyInformation,
			IN ULONG	KeyInformationLength,
			OUT PULONG	ResultLength
			);
	typedef NTENUMERATEKEY FAR * LPNTENUMERATEKEY;

	// NtDeleteValueKey
	typedef NTSTATUS(STDAPICALLTYPE NTDELETEVALUEKEY)
		(
			IN HANDLE			KeyHandle,
			IN PUNICODE_STRING	ValueName
			);
	typedef NTDELETEVALUEKEY FAR * LPNTDELETEVALUEKEY;

	// NtEnumerateValueKey
	typedef NTSTATUS(STDAPICALLTYPE NTENUMERATEVALUEKEY)
		(
			IN HANDLE	KeyHandle,
			IN ULONG	Index,
			IN KEY_VALUE_INFORMATION_CLASS KeyValueInformationClass,
			OUT PVOID	KeyValueInformation,
			IN ULONG	KeyValueInformationLength,
			OUT PULONG	ResultLength
			);
	typedef NTENUMERATEVALUEKEY FAR * LPNTENUMERATEVALUEKEY;

	// NtQueryMultipleValueKey
	typedef NTSTATUS(STDAPICALLTYPE NTQUERYMULTIPLEVALUEKEY)
		(
			IN HANDLE		KeyHandle,
			IN OUT PKEY_MULTIPLE_VALUE_INFORMATION ValuesList,
			IN ULONG		NumberOfValues,
			OUT PVOID		DataBuffer,
			IN OUT ULONG	BufferLength,
			OUT PULONG		RequiredLength			/* optional */
			);
	typedef NTQUERYMULTIPLEVALUEKEY FAR * LPNTQUERYMULTIPLEVALUEKEY;

	// NtNotifyChangeKey
	typedef NTSTATUS(STDAPICALLTYPE NTNOTIFYCHANGEKEY)
		(
			IN HANDLE				KeyHandle,
			IN HANDLE				EventHandle,
			IN PIO_APC_ROUTINE		ApcRoutine,
			IN PVOID				ApcRoutineContext,
			IN PIO_STATUS_BLOCK		IoStatusBlock,
			IN ULONG				NotifyFilter,
			IN BOOLEAN				WatchSubtree,
			OUT PVOID				RegChangesDataBuffer,
			IN ULONG				RegChangesDataBufferLength,
			IN BOOLEAN				Asynchronous
			);
	typedef NTNOTIFYCHANGEKEY FAR * LPNTNOTIFYCHANGEKEY;

	// NtRenameKey
	typedef NTSTATUS(STDAPICALLTYPE NTRENAMEKEY)
		(
			IN HANDLE			KeyHandle,
			IN PUNICODE_STRING	ReplacementName
			);
	typedef NTRENAMEKEY FAR * LPNTRENAMEKEY;


	// =================================================================
	//
	// REG_FORCE_RESTORE		Windows 2000 and later: If specified, the restore 
	//	(0x00000008L)			operation is executed even if open handles exist at or 
	//							beneath the location in the registry hierarchy the hKey 
	//							parameter points to. 
	// REG_NO_LAZY_FLUSH		If specified, the key or hive specified by the hKey 
	//	(0x00000004L)			parameter will not be lazy flushed, or flushed 
	//							automatically and regularly after an interval of time. 
	// REG_REFRESH_HIVE			If specified, the location of the hive the hKey parameter 
	//	(0x00000002L)			points to will be restored to its state immediately 
	//							following the last flush. The hive must not be lazy 
	//							flushed (by calling RegRestoreKey with REG_NO_LAZY_FLUSH 
	//							specified as the value of this parameter), the caller must 
	//							have TCB privilege, and the handle the hKey parameter 
	//							refers to must point to the root of the hive. 
	// REG_WHOLE_HIVE_VOLATILE	If specified, a new, volatile (memory only) set of 
	//	(0x00000001L)			registry information, or hive, is created. If 
	//							REG_WHOLE_HIVE_VOLATILE is specified, the key identified 
	//							by the hKey parameter must be either the HKEY_USERS or 
	//							HKEY_LOCAL_MACHINE value.  
	//
	// =================================================================
	//
	// NtRestoreKey
	typedef NTSTATUS(STDAPICALLTYPE NTRESTOREKEY)
		(
			IN HANDLE	KeyHandle,
			IN HANDLE	FileHandle,
			IN ULONG	RestoreOption
			);
	typedef NTRESTOREKEY FAR * LPNTRESTOREKEY;

	// NtSaveKey
	typedef NTSTATUS(STDAPICALLTYPE NTSAVEKEY)
		(
			IN HANDLE	KeyHandle,
			IN HANDLE	FileHandle
			);
	typedef NTSAVEKEY FAR * LPNTSAVEKEY;

	// NtLoadKey
	typedef NTSTATUS(STDAPICALLTYPE NTLOADKEY)
		(
			IN POBJECT_ATTRIBUTES DestinationKeyName,	// - and HANDLE to root key.
														//   Root can be \registry\machine 
														//   or \registry\user. 
														//   All other keys are invalid. 
			IN POBJECT_ATTRIBUTES HiveFileName			// - Hive file path and name
			);
	typedef NTLOADKEY FAR * LPNTLOADKEY;

	// NtLoadKey2
	typedef NTSTATUS(STDAPICALLTYPE NTLOADKEY2)
		(
			IN POBJECT_ATTRIBUTES DestinationKeyName,
			IN POBJECT_ATTRIBUTES HiveFileName,
			IN ULONG Flags	// Flags can be 0x0000 or REG_NO_LAZY_FLUSH (0x0004)
			);
	typedef NTLOADKEY2 FAR * LPNTLOADKEY2;

	// NtReplaceKey
	typedef NTSTATUS(STDAPICALLTYPE NTREPLACEKEY)
		(
			IN POBJECT_ATTRIBUTES	NewHiveFileName,
			IN HANDLE				KeyHandle,
			IN POBJECT_ATTRIBUTES	BackupHiveFileName
			);
	typedef NTREPLACEKEY FAR * LPNTREPLACEKEY;

	// NtUnloadKey
	typedef NTSTATUS(STDAPICALLTYPE NTUNLOADKEY)
		(
			IN POBJECT_ATTRIBUTES	DestinationKeyName
			);
	typedef NTUNLOADKEY FAR * LPNTUNLOADKEY;

	// =================================================================

	// NtClose
	typedef NTSTATUS(STDAPICALLTYPE NTCLOSE)
		(
			IN HANDLE KeyHandle
			);
	typedef NTCLOSE FAR * LPNTCLOSE;

	// =================================================================

	// NtCreateFile
	typedef NTSTATUS(STDAPICALLTYPE NTCREATEFILE)
		(
			OUT PHANDLE             FileHandle,
			IN ACCESS_MASK          DesiredAccess,
			IN POBJECT_ATTRIBUTES   ObjectAttributes,
			OUT PIO_STATUS_BLOCK    IoStatusBlock,
			IN PLARGE_INTEGER       AllocationSize,		/* optional */
			IN ULONG                FileAttributes,
			IN ULONG                ShareAccess,
			IN ULONG                CreateDisposition,
			IN ULONG                CreateOptions,
			IN PVOID                EaBuffer,			/* optional */
			IN ULONG                EaLength
			);
	typedef NTCREATEFILE FAR * LPNTCREATEFILE;

	// NtOpenThread
	typedef NTSTATUS(STDAPICALLTYPE NTOPENTHREAD)
		(
			OUT PHANDLE				ThreadHandle,
			IN ACCESS_MASK			DesiredAccess,
			IN POBJECT_ATTRIBUTES	ObjectAttributes,
			IN PCLIENT_ID			ClientId		/* optional*/
			);
	typedef NTOPENTHREAD FAR * LPNTOPENTHREAD;

	// NtOpenProcessToken
	typedef NTSTATUS(STDAPICALLTYPE NTOPENPROCESSTOKEN)
		(
			IN HANDLE               ProcessHandle,
			IN ACCESS_MASK          DesiredAccess,
			OUT PHANDLE             TokenHandle
			);
	typedef NTOPENPROCESSTOKEN FAR * LPNTOPENPROCESSTOKEN;

	// NtAdjustPrivilegesToken
	typedef NTSTATUS(STDAPICALLTYPE NTADJUSTPRIVILEGESTOKEN)
		(
			IN HANDLE               TokenHandle,
			IN BOOLEAN              DisableAllPrivileges,
			IN PTOKEN_PRIVILEGES    TokenPrivileges,
			IN ULONG                PreviousPrivilegesLength,
			OUT PTOKEN_PRIVILEGES   PreviousPrivileges,	/* optional */
			OUT PULONG              RequiredLength		/* optional */
			);
	typedef NTADJUSTPRIVILEGESTOKEN FAR * LPNTADJUSTPRIVILEGESTOKEN;

	// NtQueryInformationToken
	typedef NTSTATUS(STDAPICALLTYPE NTQUERYINFORMATIONTOKEN)
		(
			IN HANDLE TokenHandle,
			IN TOKEN_INFORMATION_CLASS TokenInformationClass,
			OUT PVOID TokenInformation,
			IN ULONG TokenInformationLength,
			OUT PULONG ReturnLength
			);
	typedef NTQUERYINFORMATIONTOKEN FAR * LPNTQUERYINFORMATIONTOKEN;

	// RtlAllocateHeap
	typedef NTSTATUS(STDAPICALLTYPE RTLALLOCATEHEAP)
		(
			IN PVOID HeapHandle,
			IN ULONG Flags,
			IN ULONG Size
			);
	typedef RTLALLOCATEHEAP FAR * LPRTLALLOCATEHEAP;

	// RtlFreeHeap
	typedef NTSTATUS(STDAPICALLTYPE RTLFREEHEAP)
		(
			IN PVOID HeapHandle,
			IN ULONG Flags,								/* optional */
			IN PVOID MemoryPointer
			);
	typedef RTLFREEHEAP FAR * LPRTLFREEHEAP;



	// NtCompactKeys
	typedef NTSTATUS(STDAPICALLTYPE NTCOMPACTKEYS)
		(
			IN ULONG NrOfKeys,
			IN HANDLE KeysArray
			);
	typedef NTCOMPACTKEYS FAR * LPNTCOMPACTKEYS;

	// NtCompressKey
	typedef NTSTATUS(STDAPICALLTYPE NTCOMPRESSKEY)
		(
			IN HANDLE Key
			);
	typedef NTCOMPRESSKEY FAR * LPNTCOMPRESSKEY;

	// NtLockRegistryKey
	typedef NTSTATUS(STDAPICALLTYPE NTLOCKREGISTRYKEY)
		(
			IN HANDLE KeyHandle
			);
	typedef NTLOCKREGISTRYKEY FAR * LPNTLOCKREGISTRYKEY;

	// NtQueryOpenSubKeysEx
	typedef NTSTATUS(STDAPICALLTYPE NTQUERYOPENSUBKEYSEX)
		(
			IN POBJECT_ATTRIBUTES TargetKey,
			IN ULONG BufferLength,
			IN PVOID Buffer,
			IN PULONG RequiredSize
			);
	typedef NTQUERYOPENSUBKEYSEX FAR * LPNTQUERYOPENSUBKEYSEX;

	// NtSaveKeyEx
	typedef NTSTATUS(STDAPICALLTYPE NTSAVEKEYEX)
		(
			IN HANDLE KeyHandle,
			IN HANDLE FileHandle,
			IN ULONG Flags
			);
	typedef NTSAVEKEYEX FAR * LPNTSAVEKEYEX;

	// NtLoadKeyEx
	typedef NTSTATUS(STDAPICALLTYPE NTLOADKEYEX)
		(
			IN POBJECT_ATTRIBUTES TargetKey,
			IN POBJECT_ATTRIBUTES SourceFile,
			IN ULONG Flags,
			IN HANDLE TrustClassKey
			);
	typedef NTLOADKEYEX FAR * LPNTLOADKEYEX;

	// NtUnloadKey2
	typedef NTSTATUS(STDAPICALLTYPE NTUNLOADKEY2)
		(
			IN POBJECT_ATTRIBUTES TargetKey,
			IN ULONG Flags
			);
	typedef NTUNLOADKEY2 FAR * LPNTUNLOADKEY2;

	// NtUnloadKeyEx
	typedef NTSTATUS(STDAPICALLTYPE NTUNLOADKEYEX)
		(
			IN POBJECT_ATTRIBUTES TargetKey,
			IN HANDLE Event
			);
	typedef NTUNLOADKEYEX FAR * LPNTUNLOADKEYEX;

	typedef enum _OBJECT_INFORMATION_CLASS {
		ObjectBasicInformation = 0,
		ObjectTypeInformation = 2
	} OBJECT_INFORMATION_CLASS;


	// NtQueryObject
	typedef NTSTATUS(STDAPICALLTYPE NTQUERYOBJECT)
		(
			_In_opt_  HANDLE                   Handle,
			_In_      OBJECT_INFORMATION_CLASS ObjectInformationClass,
			_Out_opt_ PVOID                    ObjectInformation,
			_In_      ULONG                    ObjectInformationLength,
			_Out_opt_ PULONG                   ReturnLength
			);
	typedef NTQUERYOBJECT FAR * LPNTQUERYOBJECT;


	public ref class Registry sealed
	{
	private:

		property LPGETTOKENINFORMATION			GetTokenInformation;
		property LPOPENPROCESSTOKEN				OpenProcessToken;
		property LPOPENTHREADTOKEN				OpenThreadToken;

		////////////////////////////////////////////////////////
		// Nt Native API's
		//
		// NTDLL.dll Entry Points
		//
		property LPNTCREATEKEY				NtCreateKey;
		property LPNTOPENKEY				NtOpenKey;
		property LPNTDELETEKEY				NtDeleteKey;
		property LPNTFLUSHKEY				NtFlushKey;
		property LPNTQUERYKEY				NtQueryKey;
		property LPNTENUMERATEKEY			NtEnumerateKey;
		//
		////////////////////////////////////////////////////////


		////////////////////////////////////////////////////////
		// Nt Value Functions
		////////////////////////////////////////////////////////
		//
		property LPNTSETVALUEKEY			NtSetValueKey;
		property LPNTSETINFORMATIONKEY		NtSetInformationKey;
		property LPNTQUERYVALUEKEY			NtQueryValueKey;
		property LPNTENUMERATEVALUEKEY		NtEnumerateValueKey;
		property LPNTDELETEVALUEKEY			NtDeleteValueKey;
		property LPNTQUERYMULTIPLEVALUEKEY	NtQueryMultipleValueKey;
		//
		////////////////////////////////////////////////////////


		////////////////////////////////////////////////////////
		// Nt New Functions for WinXP and Serer 2K3
		////////////////////////////////////////////////////////
		//
		// (WinXP) Renames a Registry key.
		property LPNTRENAMEKEY				NtRenameKey;
		// (WinXP) Makes key storage adjacent.
		property LPNTCOMPACTKEYS			NtCompactKeys;
		// (WinXP) Performs in-place compaction of a hive.
		property LPNTCOMPRESSKEY			NtCompressKey;
		// (WinXP) Locks a registry key for modification.
		property LPNTLOCKREGISTRYKEY		NtLockRegistryKey;
		// (Server 2K3) Returns the keys opened beneath a specified key.
		property LPNTQUERYOPENSUBKEYSEX		NtQueryOpenSubKeysEx;
		// (WinXP) Saves the contents of a key and its subkeys to a file.
		property LPNTSAVEKEYEX				NtSaveKeyEx;
		// (Server 2K3) Loads a hive into the Registry.
		property LPNTLOADKEYEX				NtLoadKeyEx;
		// (Server 2K3) Unloads a hive from the Registry.
		property LPNTUNLOADKEY2				NtUnloadKey2;
		// (WinXP) Unloads a hive from the Registry.
		property LPNTUNLOADKEYEX			NtUnloadKeyEx;
		//
		////////////////////////////////////////////////////////


		////////////////////////////////////////////////////////
		// Nt Hive Functions
		////////////////////////////////////////////////////////
		//
		property LPNTSAVEKEY				NtSaveKey;
		property LPNTRESTOREKEY				NtRestoreKey;
		property LPNTLOADKEY				NtLoadKey;
		property LPNTLOADKEY2				NtLoadKey2;
		property LPNTREPLACEKEY				NtReplaceKey;
		property LPNTUNLOADKEY				NtUnloadKey;
		//
		////////////////////////////////////////////////////////


		////////////////////////////////////////////////////////
		// Nt Misc Functions
		////////////////////////////////////////////////////////
		//
		property LPNTCLOSE					NtClose;
		property LPNTNOTIFYCHANGEKEY		NtNotifyChangeKey;
		property LPNTOPENTHREAD				NtOpenThread;
		//
		////////////////////////////////////////////////////////


		////////////////////////////////////////////////////////
		// Nt File Functions
		////////////////////////////////////////////////////////
		//
		property LPNTCREATEFILE				NtCreateFile;
		//
		////////////////////////////////////////////////////////


		////////////////////////////////////////////////////////
		// Nt Process Functions
		////////////////////////////////////////////////////////
		//
		property LPNTOPENPROCESSTOKEN		NtOpenProcessToken;
		property LPNTADJUSTPRIVILEGESTOKEN	NtAdjustPrivilegesToken;
		property LPNTQUERYINFORMATIONTOKEN	NtQueryInformationToken;
		//
		////////////////////////////////////////////////////////

		////////////////////////////////////////////////////////
		// END - Native API Functions
		////////////////////////////////////////////////////////


		////////////////////////////////////////////////////////
		// Rtl String Functions
		////////////////////////////////////////////////////////
		//
		property LPRTLINITSTRING				RtlInitString;
		property LPRTLINITANSISTRING			RtlInitAnsiString;
		property LPRTLINITUNICODESTRING			RtlInitUnicodeString;
		property LPRTLANSISTRINGTOUNICODESTRING	RtlAnsiStringToUnicodeString;
		property LPRTLUNICODESTRINGTOANSISTRING	RtlUnicodeStringToAnsiString;
		property LPRTLFREESTRING				RtlFreeString;
		property LPRTLFREEANSISTRING			RtlFreeAnsiString;
		property LPRTLFREEUNICODESTRING			RtlFreeUnicodeString;
		property LPRTLCONVERTSIDTOUNICODESTRING	RtlConvertSidToUnicodeString;
		//
		////////////////////////////////////////////////////////

		////////////////////////////////////////////////////////
		// Nt Heap Functions
		////////////////////////////////////////////////////////
		//
		property LPRTLALLOCATEHEAP			RtlAllocateHeap;
		property LPRTLFREEHEAP				RtlFreeHeap;
		//
		////////////////////////////////////////////////////////

		//property LPRTLOOKUPPRIVILEGEVALUE	LookupPrivilegeValue;

		property LPNTQUERYOBJECT	NtQueryObject;

	public:
		Registry();
		void	Registry::InitNTDLLEntryPoints();

		RegistryType Registry::GetValueInfo(RegistryHive Hive, Platform::String^ Key, Platform::String^ Name, int nSize);

		Platform::Boolean Registry::GetSubKeyList(RegistryHive Hive, Platform::String^ Key, Platform::Array<Platform::String^> ^*csaSubkeys);
		Platform::Boolean Registry::GetValueList(RegistryHive Hive, Platform::String^ Key, Platform::Array<Platform::String^> ^*csaValues);

		Platform::Boolean Registry::ValueExists(RegistryHive Hive, Platform::String^ Key, Platform::String^ Name);

		Platform::Boolean Registry::WriteValue(RegistryHive Hive, Platform::String^ Key, Platform::String^ Name, const Platform::Array<uint8>^ csaValue, RegistryType type);
		Platform::Boolean Registry::QueryValue(RegistryHive Hive, Platform::String^ Key, Platform::String^ Name, RegistryType* RegType, Platform::Array<uint8>^* RetBuffer);

		Platform::Boolean Registry::RenameKey(RegistryHive Hive, Platform::String^ Key, Platform::String^ csNewKeyName);

		Platform::Boolean Registry::DeleteKey(RegistryHive Hive, Platform::String^ Key);
		Platform::Boolean Registry::DeleteKeysRecursive(RegistryHive Hive, Platform::String^ Key);

		Platform::Boolean Registry::CreateKey(RegistryHive Hive, Platform::String^ Key);

		Platform::Boolean Registry::DeleteValue(RegistryHive Hive, Platform::String^ Key, Platform::String^ Name);
		unsigned int Registry::GetKeyStatus(RegistryHive Hive, Platform::String^ Key);
		Platform::Boolean Registry::FindHiddenKeys(RegistryHive Hive, Platform::String^ Key, Platform::Array<Platform::String^> ^*csaSubkeys);
		Platform::Boolean Registry::IsKeyHidden(RegistryHive Hive, Platform::String^ Key);
		Platform::Boolean Registry::GetKeyLastWriteTime(RegistryHive Hive, Platform::String^ Key, int64 *LastWriteTime);

		//Customs
		Platform::Boolean Registry::WriteValue(RegistryHive Hive, Platform::String^ Key, Platform::String^ Name, const Platform::Array<uint8>^ csaValue, uint32 type);
		unsigned int Registry::GetValueInfo2(RegistryHive Hive, Platform::String^ Key, Platform::String^ Name, int nSize);
		Platform::Boolean Registry::QueryValue(RegistryHive Hive, Platform::String^ Key, Platform::String^ Name, uint32* RegType, Platform::Array<uint8>^* RetBuffer);

		Platform::Boolean Registry::LoadHive(Platform::String^ HiveFile, Platform::String^ MountName, Platform::Boolean InUser);
		Platform::Boolean Registry::UnloadHive(Platform::String^ KeyPath, Platform::Boolean InUser);

	private:
		Platform::String^ Registry::GetCurrentUserSID();
		Platform::String^ Registry::GetRootPathFor(RegistryHive hRoot);
		BOOL Registry::ReadValue(RegistryHive Hive, Platform::String^ Key, Platform::String^ Name, RegistryType RegType, KEY_VALUE_PARTIAL_INFORMATION** retInfo);
		Platform::Boolean Registry::WriteValue(RegistryHive Hive, Platform::String^ Key, Platform::String^ Name, PVOID pValue, ULONG ulValueLength, RegistryType dwRegType);

		// Customs
		Platform::Boolean Registry::WriteValue(RegistryHive Hive, Platform::String^ Key, Platform::String^ Name, PVOID pValue, RegistryRT::ULONG ulValueLength, DWORD dwRegType);
		BOOL Registry::ReadValue(RegistryHive Hive, Platform::String^ Key, Platform::String^ Name, uint32 RegType, KEY_VALUE_PARTIAL_INFORMATION** retInfo);

		/*BOOL Registry::SaveKey(RegistryHive Hive, Platform::String^ Key, Platform::String^ csHiveFile);
		BOOL Registry::RestoreKey(RegistryHive Hive, Platform::String^ Key, Platform::String^ csHiveFile);
		BOOL Registry::SaveRestoreKey(RegistryHive Hive, Platform::String^ Key, Platform::String^ csHiveFile, BOOL bSaveKey);
		NTSTATUS Registry::EnablePrivilege(Platform::String^ csPrivilege, BOOL bEnable);*/
	};
}
