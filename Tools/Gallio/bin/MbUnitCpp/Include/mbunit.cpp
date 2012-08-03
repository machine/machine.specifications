// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan de Halleux
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#include "stdafx.h"
#include <stdio.h>
#include <stdlib.h>
#include <stdarg.h>
#include <wchar.h>
#include <math.h>
#include <time.h>
#include <exception>
#include "mbunit.h"

#pragma warning (disable: 4996 4355) // Hide some warnings.

namespace mbunit
{
	// =========
	// Internals
	// =========

	String::String() : buffer(0), length(-1)
	{
		Clear();
	}

	String::~String()
	{
		if (buffer != 0)
			delete[] buffer;
	}

	String::String(const String& rhs) : buffer(0), length(-1)
	{
		AppendImpl(rhs.buffer, rhs.length);
	}

	String::String(const char* str) : buffer(0), length(-1)
	{
		AppendImpl(str);
	}

	String::String(const wchar_t* wstr) : buffer(0), length(-1)
	{
		AppendImpl(wstr, (int)wcslen(wstr));
	}

	void String::Clear()
	{
		if (length != 0)
		{
			if (buffer != 0)
				delete[] buffer;

			buffer = new wchar_t[1];
			buffer[0] = L'\0';
			length = 0;
		}
	}

	void String::AppendImpl(const wchar_t* wstr, int n)
	{
        if (n < 0)
            n = (int)wcslen(wstr);

		if ((n > 0) || (length < 0))
		{
            if (length < 0)
                length = 0;

			wchar_t* newBuffer = new wchar_t[length + n + 1];

			if (length > 0)
				wcsncpy(newBuffer, buffer, length);

			if (n > 0)
				wcsncpy(newBuffer + length, wstr, n);

			length += n;
			newBuffer[length] = L'\0';

			if (buffer != 0)
				delete[] buffer;

			buffer = newBuffer;
		}
	}

	void String::AppendImpl(const char* str)
	{
		int n = (int)mbstowcs(0, str, INT_MAX);
		wchar_t* tmp = new wchar_t[n + 1];
		mbstowcs(tmp, str, n);
		AppendImpl(tmp, n);
		delete[] tmp;
	}

	#define _Impl_StringAppend(TYPE, IMPL) \
		template<> String& String::Append<TYPE>(TYPE arg) \
		{ \
			IMPL ; \
			return *this; \
		}

	_Impl_StringAppend(const String&, AppendImpl(arg.buffer, arg.length))
	_Impl_StringAppend(String, AppendImpl(arg.buffer, arg.length))
	_Impl_StringAppend(const char*, AppendImpl(arg))
	_Impl_StringAppend(const wchar_t*, AppendImpl(arg, (int)wcslen(arg)))
	_Impl_StringAppend(int, AppendFormat(L"%d", arg))
	_Impl_StringAppend(bool, AppendImpl(arg ? "true" : "false"))
	_Impl_StringAppend(char, AppendFormat("%c", arg))
	_Impl_StringAppend(wchar_t, AppendFormat(L"%lc", arg))
	_Impl_StringAppend(unsigned char, AppendFormat("%u", arg))
	_Impl_StringAppend(short, AppendFormat("%d", arg))
	_Impl_StringAppend(unsigned short, AppendFormat("%u", arg))
	_Impl_StringAppend(unsigned int,AppendFormat("%u", arg))
	_Impl_StringAppend(long, AppendFormat("%ld", arg))
	_Impl_StringAppend(unsigned long, AppendFormat("%lu", arg))
	_Impl_StringAppend(long long, AppendFormat("%ld", arg))
	_Impl_StringAppend(unsigned long long, AppendFormat("%lu", arg))
	_Impl_StringAppend(float, AppendFormat("%f", arg))
	_Impl_StringAppend(double, AppendFormat("%Lf", arg))
	_Impl_StringAppend(char*, AppendImpl(arg))
	_Impl_StringAppend(wchar_t*, AppendImpl(arg, (int)wcslen(arg)))
	_Impl_StringAppend(void*, AppendFormat(L"%lu", arg))

    #ifdef _AFX
	_Impl_StringAppend(CString, AppendImpl(arg.GetBuffer()))
    #endif

	String& String::AppendFormat(const char* format, ...)
	{
		va_list args;
		va_start(args, format);
		AppendFormat(format, args);
		va_end(args);
		return *this;
	}

	String& String::AppendFormat(const wchar_t* format, ...)
	{
		va_list args;
		va_start(args, format);
		AppendFormat(format, args);
		va_end(args);
		return *this;
	}

	String& String::AppendFormat(const wchar_t* format, va_list args)
	{
		int n = _vscwprintf(format, args);
		wchar_t* tmp = new wchar_t[n + 1];
		vswprintf(tmp, format, args);
		AppendImpl(tmp, n);
		delete[] tmp;
		return *this;
	}

	String& String::AppendFormat(const char* format, va_list args)
	{
		int n = _vscprintf(format, args);
		char* tmp = new char[n + 1];
		vsprintf(tmp, format, args);
		AppendImpl(tmp);
		delete[] tmp;
		return *this;
	}

	String String::Format(const char* format, ...)
	{
		va_list args;
		va_start(args, format);
		String str;
		str.AppendFormat(format, args);
		va_end(args);
		return str;
	}

	String String::Format(const wchar_t* format, ...)
	{
		va_list args;
		va_start(args, format);
		String str;
		str.AppendFormat(format, args);
		va_end(args);
		return str;
	}

	StringMap::StringMap()
		: head(0), nextId(1)
	{
	}

	StringMap::~StringMap()
	{
		RemoveAll();
	}

	void StringMap::RemoveAll()
	{
		StringMapNode* current = head;

		while (current != 0)
		{
			StringMapNode* next = current->Next;
			delete current->Str;
			delete current;
			current = next;
		}

		head = 0;
	}

	String* StringMap::Get(StringId key)
	{
		StringMapNode* current = head;

		while (current != 0)
		{
			if (current->Key == key)
				return current->Str;
            
			current = current->Next;
		}

		throw "Key not found.";
	}

	StringId StringMap::Add(String* str)
	{
		if (str == 0)
			return 0;

		StringMapNode* node = new StringMapNode;
		node->Key = nextId;
		node->Str = str;
		node->Next = head;
		head = node;
		return nextId++;
	}

	void StringMap::Remove(StringId key)
	{
		StringMapNode* previous = 0;
		StringMapNode* current = head;

		while (current != 0)
		{
			if (current->Key == key)
			{
				if (previous != 0)
					previous->Next = current->Next;
				else
					head = current->Next;

				delete current->Str;
				delete current;
				return;
			}
            
			previous = current;
			current = current->Next;
		}
	}

    // Construct a base test or test fixture instance.
	DecoratorTarget::DecoratorTarget(int metadataPrototypeId)
		: metadataId(0)
	{
		if (metadataPrototypeId > 0)
		{
			StringMap& map = TestFixture::GetStringMap();
			String* s = map.Get(metadataPrototypeId);
			metadataId = map.Add(new String(*s));
		}
	}
		
	// Attaches a key/value metadata to the current test or test fixture.
	void DecoratorTarget::SetMetadata(const wchar_t* key, const String& value)
	{
		String s(value);
		AppendTo(metadataId, String::Format(L"%s={%s},", key, s.GetBuffer()));
	}

	// Create a new string ID or append the specified text if it already exists.
	void DecoratorTarget::AppendTo(int& id, const String& s)
	{
		StringMap& map = TestFixture::GetStringMap();

		if (id == 0)
		{
			id = map.Add(new String(s));
		}
		else
		{
			map.Get(id)->Append(s);
		}
	}

    // Construct an executable test case.
    Test::Test(TestFixture* testFixture, const wchar_t* name, const wchar_t* fileName, int lineNumber)
        : index(testFixture->GetTestList().GetNextIndex())
		, DecoratorTarget(testFixture->GetMetadataId())
		, name(name)
		, fileName(fileName)
		, lineNumber(lineNumber)
		, testLogId(0)
		, dataSource(0)
		, hasLateFailure(false)
    {
	}

	// Desctructor.
    Test::~Test()
    {
		if (dataSource != 0)
			delete dataSource;
    }

    // Specifies the next test of the chained list.
    void Test::SetNext(Test* test)
    {
        next = test;
    }

    // Runs the current test and captures the failure(s).
    void Test::Run(TestResultData* testResultData, void* dataRow)
    {
		BeforeRun();
#ifdef MBUNITCPP_SUPPORT_GOOGLE_MOCK
		GoogleMockRegistration::GetInstance().Run(this);
#endif
		clock_t started = clock();
		bool unhandledException = false;

        try
        {
            Clear();
			BindDataRow(dataRow);
            RunWithCustomExceptionHandler();
            testResultData->NativeOutcome = Passed;
		}
        catch (AssertionFailure failure)
        {
            testResultData->NativeOutcome = Failed;
            testResultData->Failure = failure;
        }
		catch (char* e)
		{
            testResultData->NativeOutcome = Failed;
            testResultData->Failure = AssertionFailure::FromException(e, "char*");
			unhandledException = true;
		}
		catch (std::exception e)
		{
            testResultData->NativeOutcome = Failed;
            testResultData->Failure = AssertionFailure::FromException(e.what(), "std::exception");
			unhandledException = true;
		}
		catch (...)
		{
            testResultData->NativeOutcome = Failed;
            testResultData->Failure = AssertionFailure::FromException();
			unhandledException = true;
		}

		if (hasLateFailure && !unhandledException)
		{
			testResultData->NativeOutcome = Failed;
			testResultData->Failure = lateFailure;
		}

		testResultData->DurationMilliseconds = 1000 * (clock() - started) / CLOCKS_PER_SEC;
		testResultData->TestLogId = testLogId;
        testResultData->AssertCount = assertCount;
    }

	void Test::RunWithCustomExceptionHandler()
	{
		RunImpl(); // Default does not handle with any custom exception.
	}

	// Clears internal variables for new run.
    void Test::Clear()
    {
        assertCount = 0;
		testLogId = 0;
    }

	// Increment the assertion count by 1.
    void Test::IncrementAssertCount()
    {
        assertCount++;
    }

	// Appends the specified text to the test log.
	void Test::AppendToTestLog(const String& s)
	{
		AppendTo(testLogId, s);
	}

    // Default empty implementation of the test execution.
    void Test::RunImpl()
    {
    }

	// Binds the specified data source to the test instance.
	void Test::Bind(AbstractDataSource* dataSource)
	{
		this->dataSource = dataSource;
	}

	// Binds the specified data row to the test step.
	void Test::BindDataRow(void* dataRow) 
	{
	}

	void Test::SetLateFailure(const AssertionFailure& lateFailure)
	{
		if (!hasLateFailure)
		{
			this->lateFailure = lateFailure;
			hasLateFailure = true;
		}
	}

    // Constructs an empty list of tests.
    TestList::TestList()
        : head(0), tail(0), nextIndex(0)
    {
    }

    // Adds a new test at the end of the list.
    void TestList::Add(Test* test)
    {
        if (tail == 0)
            head = test;
        else
            tail->SetNext(test);
        
        tail = test;
    }

    // Returns the next unused test ID.
    int TestList::GetNextIndex()
    {
        return nextIndex ++;
    }

    // Constructs a test fixture.
    TestFixture::TestFixture(int index, const wchar_t* name)
        : index(index), name(name)
    {
    }

    TestFixture::~TestFixture()
    {
    }

    // Specifies the next test fixture of the chained list.
    void TestFixture::SetNext(TestFixture* testFixture)
    {
        next = testFixture;
    }

    // Returns the list of tests defined in the current test fixture.
    TestList& TestFixture::GetTestList()
    {
        return children;
    }

    // Gets the singleton list of test fixtures.
    TestFixtureList& TestFixture::GetTestFixtureList()
    {
        static TestFixtureList list;
        return list;
    }

	// Gets the singleton map of strings.
	StringMap& TestFixture::GetStringMap()
	{
	    static StringMap map;
        return map;
	}

    // Constructs an empty list of test fixtures.
    TestFixtureList::TestFixtureList()
        : head(0), tail(0), nextIndex(0)
    {
    }

    // Adds a new test fixture at the end of the list.
    void TestFixtureList::Add(TestFixture* testFixture)
    {
        if (tail == 0)
            head = testFixture;
        else
            tail->SetNext(testFixture);
        
        tail = testFixture;
    }

    // Gets the next unused test fixture ID.
    int TestFixtureList::GetNextIndex()
    {
        return nextIndex ++;
    }

    // Registers the specified test in the list.
    TestRecorder::TestRecorder(TestList& list, Test* test)
    {
        list.Add(test);
    }

    // Registers the specified test fixture in the list.
    TestFixtureRecorder::TestFixtureRecorder(TestFixtureList& list, TestFixture* testFixture)
    {
        list.Add(testFixture);
    }

    // A structure describing the currently enumerated test or test fixture.
    struct Position
    {
        TestFixture* TestFixture;
        Test* Test;
		void* DataRow;
    };

	// Type of the curent test.
	enum TestKind
	{
		KindFixture = 0,
        KindTest = 1,
        KindGroup = 2,
		KindRowTest = 3,
	};

    // A portable structure to describe the current test or test fixture.
    struct TestInfoData
    {
        const wchar_t* Name;
        int Index;
        TestKind Kind;
        const wchar_t* FileName;
        int LineNumber;
        Position Position;
		int MetadataId;
    };

	// Constructs an empty assertion failure.
	AssertionFailure::AssertionFailure()
		: DescriptionId(0),  MessageId(0), LineNumber(0)
	{
	}

	// Constructs an empty labeled value.
	LabeledValue::LabeledValue()
		: LabelId(0), ValueId(0), ValueType(TypeRaw)
	{
	}

	// Creates an assertion failure for an unhandled exception.
	AssertionFailure AssertionFailure::FromException(const char* exceptionMessage, const char* exceptionType)
	{
		StringMap& map = TestFixture::GetStringMap();
		AssertionFailure failure;
		failure.DescriptionId = (exceptionType == 0)
			? map.Add(new String(L"An unhandled exception was thrown."))
			: map.Add(new String(String::Format("An unhandled exception of type '%s' was thrown.", exceptionType)));
		failure.MessageId = map.Add(exceptionMessage == 0 ? 0 : new String(exceptionMessage));
		return failure;
	}

	// Initialize a labeled value.
	void LabeledValue::Set(StringId valueId, mbunit::ValueType valueType, StringId labelId)
	{
		ValueId = valueId;
		ValueType = valueType;
		LabelId = labelId;
	}

	// Default constructor for abstract data sources.
	AbstractDataSource::AbstractDataSource()
		: head(0)
	{
	}

	// Stores the head data row.
	void AbstractDataSource::SetHead(void* dataRow)
	{
		head = dataRow;
	}

	// ===================
	// Google Mock Support
	// ===================

#ifdef MBUNITCPP_SUPPORT_GOOGLE_MOCK

	GoogleMockListener::GoogleMockListener()
		: test(0)
	{
	}

	void GoogleMockListener::OnTestPartResult(const testing::TestPartResult& test_part_result) 
	{
		if (test_part_result.failed())
		{
			StringMap& map = TestFixture::GetStringMap();
			AssertionFailure failure;
			failure.DescriptionId = map.Add(new String(test_part_result.summary()));
			failure.LineNumber = test_part_result.line_number();
			test->SetLateFailure(failure);
		}
	}
    
	void GoogleMockListener::SetTest(Test* test)
	{
		this->test = test;
	}

	GoogleMockRegistration::GoogleMockRegistration()
		: registered(false), listener(0)
	{
	}

	void GoogleMockRegistration::Run(Test* test)
	{
		if (!registered)
		{
			int argc = 0;
			char* argv = "";
			testing::InitGoogleMock(&argc, &argv);
			testing::TestEventListeners& listeners = testing::UnitTest::GetInstance()->listeners();
			listener = new GoogleMockListener;
			listeners.Append(listener);
			registered = true;
		}

		listener->SetTest(test);
	}

    GoogleMockRegistration& GoogleMockRegistration::GetInstance()
    {
        static GoogleMockRegistration instance;
        return instance;
    }

#endif

	// =============
	// Log Recording
	// =============

	TestLogRecorder::TestLogRecorder()
		: test(0)
	{
	}
	
	void TestLogRecorder::SetTest(Test* test)
	{
		this->test = test;
	}

    void TestLogRecorder::Write(const String& str)
    {
		test->AppendToTestLog(str);
    }

    void TestLogRecorder::WriteLine(const String& str)
    {
		test->AppendToTestLog(str);
		test->AppendToTestLog(String(L"\r\n"));
    }

    void TestLogRecorder::WriteFormat(const char* format, ...)
    {
		va_list args;
		va_start(args, format);
        String str;
        str.AppendFormat(format, args);
		va_end(args);
		test->AppendToTestLog(str);
    }

    void TestLogRecorder::WriteFormat(const wchar_t* format,...)
    {
		va_list args;
		va_start(args, format);
        String str;
        str.AppendFormat(format, args);
		va_end(args);
		test->AppendToTestLog(str);
    }

    void TestLogRecorder::WriteLineFormat(const char* format, ...)
    {
		va_list args;
		va_start(args, format);
        String str;
        str.AppendFormat(format, args);
		va_end(args);
		test->AppendToTestLog(str);
		test->AppendToTestLog(String(L"\r\n"));
    }

    void TestLogRecorder::WriteLineFormat(const wchar_t* format, ...)
    {
		va_list args;
		va_start(args, format);
        String str;
        str.AppendFormat(format, args);
		va_end(args);
		test->AppendToTestLog(str);
		test->AppendToTestLog(String(L"\r\n"));
    }

#ifdef _AFX

    void TestLogRecorder::Write(CString& str)
    {
		test->AppendToTestLog(str.GetBuffer());
		str.ReleaseBuffer();
    }

    void TestLogRecorder::WriteLine(CString& str)
    {
		Write(str);
		test->AppendToTestLog(String(L"\r\n"));
    }

#endif

	// ===================
	// Assertion Framework 
	// ===================

	StringId AssertionFrameworkController::AddNewString(const char* str)
	{
		return Map().Add((str == 0) ? 0 :  new String(str));
	}

	StringId AssertionFrameworkController::AddNewString(const wchar_t* wstr)
	{
		return Map().Add((wstr == 0) ? 0 : new String(wstr));
	}

	StringId AssertionFrameworkController::AddNewString(const String& str)
    {
		return (str.GetLength() == 0) ? 0 : Map().Add(new String(str));
    }

	StringMap& AssertionFrameworkController::Map() const
	{ 
		return TestFixture::GetStringMap(); 
	}

	AssertionFrameworkController::AssertionFrameworkController()
		: lineNumber(0), test(0), framework(this)
	{
	}
		
	AssertionFramework& AssertionFrameworkController::Get(int lineNumber)
	{
		this->lineNumber = lineNumber;
		return framework;
	}

    // Internal assert count increment.
    void AssertionFrameworkController::IncrementAssertCount()
    {
        test->IncrementAssertCount();
    }

	void AssertionFrameworkController::SetTest(Test* test)
	{
		this->test = test;
	}

    // =========================
    // Assertion failure builder
    // =========================

    AssertionFailureBuilder::AssertionFailureBuilder(const String& description)
        : description(description), message(0), expected(0), actual(0), unexpected(0), 
        extraLabel0(0), extraValue0(0), extraLabel1(0), extraValue1(0)
    {
    }

    AssertionFailureBuilder::~AssertionFailureBuilder()
    {
        if (message != 0)
            delete message;
        if (expected != 0)
            delete expected;
        if (actual != 0)
            delete actual;
        if (unexpected != 0)
            delete unexpected;
        if (extraLabel0 != 0)
            delete extraLabel0;
        if (extraValue0 != 0)
            delete extraValue0;
        if (extraLabel1 != 0)
            delete extraLabel1;
        if (extraValue1 != 0)
            delete extraValue1;
    }

    AssertionFailureBuilder& AssertionFailureBuilder::Message(const String& message)
    {
        this->message = new String(message);
        return *this;
    }

    AssertionFailureBuilder& AssertionFailureBuilder::Expected(const String& expected, ValueType type)
    {
        this->expected = new String(expected);
        expectedType = type;
        return *this;
    }

    AssertionFailureBuilder& AssertionFailureBuilder::Actual(const String& actual, ValueType type)
    {
        this->actual = new String(actual);
        actualType = type;
        return *this;
    }

    AssertionFailureBuilder& AssertionFailureBuilder::Unexpected(const String& unexpected, ValueType type)
    {
        this->unexpected = new String(unexpected);
        unexpectedType = type;
        return *this;
    }

    AssertionFailureBuilder& AssertionFailureBuilder::Extra_0(const String& label, const String& value, ValueType type)
    {
        this->extraLabel0 = new String(label);
        this->extraValue0 = new String(value);
        extraType0 = type;
        return *this;
    }

    AssertionFailureBuilder& AssertionFailureBuilder::Extra_1(const String& label, const String& value, ValueType type)
    {
        this->extraLabel1 = new String(label);
        this->extraValue1 = new String(value);
        extraType1 = type;
        return *this;
    }

    AssertionFailure AssertionFailureBuilder::ToAssertionFailure(AssertionFrameworkController* controller)
    {
        AssertionFailure failure;
        failure.DescriptionId = controller->AddNewString(description);
        
        if (expected != 0)
            failure.Expected.Set(controller->AddNewString(*expected), expectedType);
        if (actual != 0)
            failure.Actual.Set(controller->AddNewString(*actual), actualType);
        if (unexpected != 0)
            failure.Unexpected.Set(controller->AddNewString(*unexpected), unexpectedType);
        if (message != 0)
            failure.MessageId = controller->AddNewString(*message);
        if (extraLabel0 != 0)
            failure.Extra_0.Set(controller->AddNewString(*extraValue0), extraType0, controller->AddNewString(*extraLabel0));
        if (extraLabel1 != 0)
            failure.Extra_1.Set(controller->AddNewString(*extraValue1), extraType1, controller->AddNewString(*extraLabel1));

        failure.LineNumber = controller->GetLineNumber();
        return failure;
    }

    // ===================
	// Assertion Framework 
	// ===================

	// Constructs an assertion framework instance for the specified controller.
    AssertionFramework::AssertionFramework(AssertionFrameworkController* controller)
        : controller(controller)
    {
    }

    // Assertion that makes inconditionally the test fail.
    void AssertionFramework::Fail(const String& message)
    {
        controller->IncrementAssertCount();
        throw AssertionFailureBuilder("An assertion failed.")
            .Message(message)
            .ToAssertionFailure(controller);
    }

	// Asserts that the specified boolean value is true.
	void AssertionFramework::IsTrue(bool actualValue, const String& message)
	{
		controller->IncrementAssertCount();
		if (!actualValue)
		{
            throw AssertionFailureBuilder("Expected value to be true.")
                .Actual("false", TypeBoolean)
                .Message(message)
                .ToAssertionFailure(controller);
		}
	}

	// Asserts that the specified boolean value is false.
	void AssertionFramework::IsFalse(bool actualValue, const String& message)
	{
		controller->IncrementAssertCount();
		if (actualValue)
		{
            throw AssertionFailureBuilder("Expected value to be false.")
                .Actual("true", TypeBoolean)
                .Message(message)
                .ToAssertionFailure(controller);
		}
	}

	// Asserts that the expected value and the actual value are equivalent.
    #define _Impl_AssertionFramework_AreEqual(TYPE, CONDITION, MANAGEDTYPE) \
		template<> void AssertionFramework::AreEqual<TYPE>(TYPE expectedValue, TYPE actualValue, const String& message) \
		{ \
			controller->IncrementAssertCount(); \
			if (CONDITION) \
			{ \
                String expected, actual; \
                expected.Append(expectedValue); \
                actual.Append(actualValue); \
                throw AssertionFailureBuilder("Expected values to be equal.") \
                    .Actual(actual, MANAGEDTYPE) \
                    .Expected(expected, MANAGEDTYPE) \
                    .Message(message) \
                    .ToAssertionFailure(controller); \
			} \
		}

    _Impl_AssertionFramework_AreEqual(bool, expectedValue != actualValue, TypeBoolean)
    _Impl_AssertionFramework_AreEqual(char, expectedValue != actualValue, TypeChar)
    _Impl_AssertionFramework_AreEqual(wchar_t, expectedValue != actualValue, TypeChar)
    _Impl_AssertionFramework_AreEqual(unsigned char, expectedValue != actualValue, TypeByte)
    _Impl_AssertionFramework_AreEqual(short, expectedValue != actualValue, TypeInt16)
    _Impl_AssertionFramework_AreEqual(unsigned short, expectedValue != actualValue, TypeUInt16)
    _Impl_AssertionFramework_AreEqual(int, expectedValue != actualValue, TypeInt32)
    _Impl_AssertionFramework_AreEqual(unsigned int, expectedValue != actualValue, TypeUInt32)
    _Impl_AssertionFramework_AreEqual(long, expectedValue != actualValue, TypeUInt64)
    _Impl_AssertionFramework_AreEqual(unsigned long, expectedValue != actualValue, TypeUInt64)
    _Impl_AssertionFramework_AreEqual(long long, expectedValue != actualValue, TypeUInt64)
    _Impl_AssertionFramework_AreEqual(unsigned long long, expectedValue != actualValue, TypeUInt64)
    _Impl_AssertionFramework_AreEqual(float, expectedValue != actualValue, TypeSingle)
    _Impl_AssertionFramework_AreEqual(double, expectedValue != actualValue, TypeDouble)
    _Impl_AssertionFramework_AreEqual(char*, strcmp(expectedValue, actualValue) != 0, TypeString)
    _Impl_AssertionFramework_AreEqual(const char*, strcmp(expectedValue, actualValue) != 0, TypeString)
    _Impl_AssertionFramework_AreEqual(wchar_t*, wcscmp(expectedValue, actualValue) != 0, TypeString)
    _Impl_AssertionFramework_AreEqual(const wchar_t*, wcscmp(expectedValue, actualValue) != 0, TypeString)
    _Impl_AssertionFramework_AreEqual(void*, expectedValue != actualValue, TypeAddress)

    #ifdef _AFX
    _Impl_AssertionFramework_AreEqual(CString, (expectedValue).Compare(actualValue) != 0, TypeString)
    #endif

	// Asserts that the unexpected value and the actual value are not equivalent.
    #define _Impl_AssertionFramework_AreNotEqual(TYPE, CONDITION, MANAGEDTYPE) \
		template<> void AssertionFramework::AreNotEqual<TYPE>(TYPE unexpectedValue, TYPE actualValue, const String& message) \
		{ \
			controller->IncrementAssertCount(); \
			if (CONDITION) \
			{ \
                String unexpected, actual; \
                unexpected.Append(unexpectedValue); \
                actual.Append(actualValue); \
                throw AssertionFailureBuilder("Expected values to be non-equal.") \
                    .Actual(actual, MANAGEDTYPE) \
                    .Unexpected(unexpected, MANAGEDTYPE) \
                    .Message(message) \
                    .ToAssertionFailure(controller); \
			} \
		}

    _Impl_AssertionFramework_AreNotEqual(bool, unexpectedValue == actualValue, TypeBoolean)
    _Impl_AssertionFramework_AreNotEqual(char, unexpectedValue == actualValue, TypeChar)
    _Impl_AssertionFramework_AreNotEqual(wchar_t, unexpectedValue == actualValue, TypeChar)
    _Impl_AssertionFramework_AreNotEqual(unsigned char, unexpectedValue == actualValue, TypeByte)
    _Impl_AssertionFramework_AreNotEqual(short, unexpectedValue == actualValue, TypeInt16)
    _Impl_AssertionFramework_AreNotEqual(unsigned short, unexpectedValue == actualValue, TypeUInt16)
    _Impl_AssertionFramework_AreNotEqual(int, unexpectedValue == actualValue, TypeInt32)
    _Impl_AssertionFramework_AreNotEqual(unsigned int, unexpectedValue == actualValue, TypeUInt32)
    _Impl_AssertionFramework_AreNotEqual(long, unexpectedValue == actualValue, TypeUInt64)
    _Impl_AssertionFramework_AreNotEqual(unsigned long, unexpectedValue == actualValue, TypeUInt64)
    _Impl_AssertionFramework_AreNotEqual(long long, unexpectedValue == actualValue, TypeUInt64)
    _Impl_AssertionFramework_AreNotEqual(unsigned long long, unexpectedValue == actualValue, TypeUInt64)
    _Impl_AssertionFramework_AreNotEqual(float, unexpectedValue == actualValue, TypeSingle)
    _Impl_AssertionFramework_AreNotEqual(double, unexpectedValue == actualValue, TypeDouble)
    _Impl_AssertionFramework_AreNotEqual(char*, strcmp(unexpectedValue, actualValue) == 0, TypeString)
    _Impl_AssertionFramework_AreNotEqual(const char*, strcmp(unexpectedValue, actualValue) == 0, TypeString)
    _Impl_AssertionFramework_AreNotEqual(wchar_t*, wcscmp(unexpectedValue, actualValue) == 0, TypeString)
    _Impl_AssertionFramework_AreNotEqual(const wchar_t*, wcscmp(unexpectedValue, actualValue) == 0, TypeString)
    _Impl_AssertionFramework_AreNotEqual(void*, unexpectedValue == actualValue, TypeAddress)

    #ifdef _AFX
    _Impl_AssertionFramework_AreNotEqual(CString, (unexpectedValue).Compare(actualValue) != 0, TypeString)
    #endif

    // Asserts that the expected value and the actual value are approximately equal.
    #define _Impl_AssertionFramework_AreApproximatelyEqual(TYPE, CONDITION, MANAGEDTYPE) \
		template<> void AssertionFramework::AreApproximatelyEqual<TYPE>(TYPE expectedValue, TYPE actualValue, TYPE delta, const String& message) \
		{ \
			controller->IncrementAssertCount(); \
			if (CONDITION) \
			{ \
                String expected, actual, deltas; \
                expected.Append(expectedValue); \
                actual.Append(actualValue); \
                deltas.Append(delta); \
                throw AssertionFailureBuilder("Expected values to be approximately equal to within a delta.") \
                    .Actual(actual, MANAGEDTYPE) \
                    .Expected(expected, MANAGEDTYPE) \
                    .Extra_0("Delta", deltas, MANAGEDTYPE) \
                    .Message(message) \
                    .ToAssertionFailure(controller); \
			} \
		}

    _Impl_AssertionFramework_AreApproximatelyEqual(char, abs(expectedValue - actualValue) > delta, TypeChar)
    _Impl_AssertionFramework_AreApproximatelyEqual(wchar_t, abs(expectedValue - actualValue) > delta, TypeChar)
    _Impl_AssertionFramework_AreApproximatelyEqual(unsigned char, abs((short)expectedValue - (short)actualValue) > (short)delta, TypeByte)
    _Impl_AssertionFramework_AreApproximatelyEqual(short, abs(expectedValue - actualValue) > delta, TypeInt16)
    _Impl_AssertionFramework_AreApproximatelyEqual(unsigned short, abs((int)expectedValue - (int)actualValue) > (int)delta, TypeUInt16)
    _Impl_AssertionFramework_AreApproximatelyEqual(int, abs(expectedValue - actualValue) > delta, TypeInt32)
    _Impl_AssertionFramework_AreApproximatelyEqual(unsigned int, _abs64((long long)expectedValue - (long long)actualValue) > (long long)delta, TypeUInt32)
    _Impl_AssertionFramework_AreApproximatelyEqual(long, abs(expectedValue - actualValue) > delta, TypeInt32)
    _Impl_AssertionFramework_AreApproximatelyEqual(unsigned long, _abs64((long long)expectedValue - (long long)actualValue) > (long long)delta, TypeUInt32)
    _Impl_AssertionFramework_AreApproximatelyEqual(long long, _abs64(expectedValue - actualValue) > delta, TypeInt64)
    _Impl_AssertionFramework_AreApproximatelyEqual(unsigned long long, fabs((double)expectedValue - (double)actualValue) > (double)delta, TypeInt64)
    _Impl_AssertionFramework_AreApproximatelyEqual(float, fabs(expectedValue - actualValue) > delta, TypeSingle)
    _Impl_AssertionFramework_AreApproximatelyEqual(double, fabs(expectedValue - actualValue) > delta, TypeDouble)

	// Verifies that an actual value is null.
	void AssertionFramework::IsNull(void* actualValue, const String& message)
	{
		controller->IncrementAssertCount();
		if (actualValue != 0)
		{
            String actual;
            actual.Append(actualValue);
            throw AssertionFailureBuilder("Expected value to be null.")
                .Actual(actual, TypeAddress)
                .Message(message)
                .ToAssertionFailure(controller);
		}
	}

	// Verifies that an actual value is not null.
	void AssertionFramework::IsNotNull(void* actualValue, const String& message)
	{
		controller->IncrementAssertCount();
		if (actualValue == 0)
		{
            throw AssertionFailureBuilder("Expected value to be non-null.")
                .Message(message)
                .ToAssertionFailure(controller);
		}
	}

    // Compares the left and the right values.
    #define _Impl_AssertionFramework_CompareValues(FUNC, TYPE, CONDITION, MANAGEDTYPE, MESSAGE) \
		template<> void AssertionFramework::FUNC<TYPE>(TYPE left, TYPE right, const String& message) \
		{ \
			controller->IncrementAssertCount(); \
			if (CONDITION) \
			{ \
                String leftFormatted, rightFormatted; \
                leftFormatted.Append(left); \
                rightFormatted.Append(right); \
                throw AssertionFailureBuilder(MESSAGE) \
                    .Extra_0("Left Value", leftFormatted, MANAGEDTYPE) \
                    .Extra_1("Right Value", rightFormatted, MANAGEDTYPE) \
                    .Message(message) \
                    .ToAssertionFailure(controller); \
			} \
		}

    #define _Impl_AssertionFramework_GreaterThan(TYPE, CONDITION, MANAGEDTYPE) \
		_Impl_AssertionFramework_CompareValues(GreaterThan, TYPE, CONDITION, MANAGEDTYPE, "Expected left to be greater than right.")
	#define _Impl_AssertionFramework_GreaterThanOrEqualTo(TYPE, CONDITION, MANAGEDTYPE) \
		_Impl_AssertionFramework_CompareValues(GreaterThanOrEqualTo, TYPE, CONDITION, MANAGEDTYPE, "Expected left to be greater than or equal to right.")
	#define _Impl_AssertionFramework_LessThan(TYPE, CONDITION, MANAGEDTYPE) \
		_Impl_AssertionFramework_CompareValues(LessThan, TYPE, CONDITION, MANAGEDTYPE, "Expected left to be less than right.")
	#define _Impl_AssertionFramework_LessThanOrEqualTo(TYPE, CONDITION, MANAGEDTYPE) \
		_Impl_AssertionFramework_CompareValues(LessThanOrEqualTo, TYPE, CONDITION, MANAGEDTYPE, "Expected left to be less than or equal to right.")

    _Impl_AssertionFramework_GreaterThan(char, left <= right, TypeChar)
    _Impl_AssertionFramework_GreaterThan(wchar_t, left <= right, TypeChar)
    _Impl_AssertionFramework_GreaterThan(unsigned char, left <= right, TypeByte)
    _Impl_AssertionFramework_GreaterThan(short, left <= right, TypeInt16)
    _Impl_AssertionFramework_GreaterThan(unsigned short, left <= right, TypeUInt16)
    _Impl_AssertionFramework_GreaterThan(int, left <= right, TypeInt32)
    _Impl_AssertionFramework_GreaterThan(unsigned int, left <= right, TypeUInt32)
    _Impl_AssertionFramework_GreaterThan(long, left <= right, TypeInt32)
    _Impl_AssertionFramework_GreaterThan(unsigned long, left <= right, TypeUInt32)
    _Impl_AssertionFramework_GreaterThan(long long, left <= right, TypeInt64)
    _Impl_AssertionFramework_GreaterThan(unsigned long long, left <= right, TypeInt64)
    _Impl_AssertionFramework_GreaterThan(float, left <= right, TypeSingle)
    _Impl_AssertionFramework_GreaterThan(double, left <= right, TypeDouble)

    _Impl_AssertionFramework_GreaterThanOrEqualTo(char, left < right, TypeChar)
    _Impl_AssertionFramework_GreaterThanOrEqualTo(wchar_t, left < right, TypeChar)
    _Impl_AssertionFramework_GreaterThanOrEqualTo(unsigned char, left < right, TypeByte)
    _Impl_AssertionFramework_GreaterThanOrEqualTo(short, left < right, TypeInt16)
    _Impl_AssertionFramework_GreaterThanOrEqualTo(unsigned short, left < right, TypeUInt16)
    _Impl_AssertionFramework_GreaterThanOrEqualTo(int, left < right, TypeInt32)
    _Impl_AssertionFramework_GreaterThanOrEqualTo(unsigned int, left < right, TypeUInt32)
    _Impl_AssertionFramework_GreaterThanOrEqualTo(long, left < right, TypeInt32)
    _Impl_AssertionFramework_GreaterThanOrEqualTo(unsigned long, left < right, TypeUInt32)
    _Impl_AssertionFramework_GreaterThanOrEqualTo(long long, left < right, TypeInt64)
    _Impl_AssertionFramework_GreaterThanOrEqualTo(unsigned long long, left < right, TypeInt64)
    _Impl_AssertionFramework_GreaterThanOrEqualTo(float, left < right, TypeSingle)
    _Impl_AssertionFramework_GreaterThanOrEqualTo(double, left < right, TypeDouble)

    _Impl_AssertionFramework_LessThan(char, left >= right, TypeChar)
    _Impl_AssertionFramework_LessThan(wchar_t, left >= right, TypeChar)
    _Impl_AssertionFramework_LessThan(unsigned char, left >= right, TypeByte)
    _Impl_AssertionFramework_LessThan(short, left >= right, TypeInt16)
    _Impl_AssertionFramework_LessThan(unsigned short, left >= right, TypeUInt16)
    _Impl_AssertionFramework_LessThan(int, left >= right, TypeInt32)
    _Impl_AssertionFramework_LessThan(unsigned int, left >= right, TypeUInt32)
    _Impl_AssertionFramework_LessThan(long, left >= right, TypeInt32)
    _Impl_AssertionFramework_LessThan(unsigned long, left >= right, TypeUInt32)
    _Impl_AssertionFramework_LessThan(long long, left >= right, TypeInt64)
    _Impl_AssertionFramework_LessThan(unsigned long long, left >= right, TypeInt64)
    _Impl_AssertionFramework_LessThan(float, left >= right, TypeSingle)
    _Impl_AssertionFramework_LessThan(double, left >= right, TypeDouble)

    _Impl_AssertionFramework_LessThanOrEqualTo(char, left > right, TypeChar)
    _Impl_AssertionFramework_LessThanOrEqualTo(wchar_t, left > right, TypeChar)
    _Impl_AssertionFramework_LessThanOrEqualTo(unsigned char, left > right, TypeByte)
    _Impl_AssertionFramework_LessThanOrEqualTo(short, left > right, TypeInt16)
    _Impl_AssertionFramework_LessThanOrEqualTo(unsigned short, left > right, TypeUInt16)
    _Impl_AssertionFramework_LessThanOrEqualTo(int, left > right, TypeInt32)
    _Impl_AssertionFramework_LessThanOrEqualTo(unsigned int, left > right, TypeUInt32)
    _Impl_AssertionFramework_LessThanOrEqualTo(long, left > right, TypeInt32)
    _Impl_AssertionFramework_LessThanOrEqualTo(unsigned long, left > right, TypeUInt32)
    _Impl_AssertionFramework_LessThanOrEqualTo(long long, left > right, TypeInt64)
    _Impl_AssertionFramework_LessThanOrEqualTo(unsigned long long, left > right, TypeInt64)
    _Impl_AssertionFramework_LessThanOrEqualTo(float, left > right, TypeSingle)
    _Impl_AssertionFramework_LessThanOrEqualTo(double, left > right, TypeDouble)

	// ======================================
	// Interface functions for Gallio adapter
	// ======================================

    extern "C" 
    {
        void __cdecl MbUnitCpp_GetHeadTest(Position* position)
        {
            TestFixtureList& list = TestFixture::GetTestFixtureList();
            TestFixture* pFirstTestFixture = list.GetHead();
            position->TestFixture = pFirstTestFixture;
            position->Test = 0;
            position->DataRow = 0;
        }

        int __cdecl MbUnitCpp_GetNextTest(Position* position, TestInfoData* testInfoData)
        {
            TestFixture* testFixture = position->TestFixture;
            Test* test = position->Test;
			void* dataRow = position->DataRow;

            if (testFixture == 0)
                return 0;
            
            if (test == 0)
            {
                testInfoData->Kind = KindFixture;
                testInfoData->FileName = 0;
                testInfoData->LineNumber = 0;
                testInfoData->Name = testFixture->GetName();
                testInfoData->Index = testFixture->GetIndex();
                testInfoData->Position.TestFixture = testFixture;
                testInfoData->Position.Test = 0;
                testInfoData->Position.DataRow = 0;
                testInfoData->MetadataId = 0;
                position->Test = testFixture->GetTestList().GetHead();
                return 1;            
            }

		    testInfoData->FileName = test->GetFileName();
			testInfoData->LineNumber = test->GetLineNumber();
			testInfoData->Index = test->GetIndex();
			testInfoData->Position.TestFixture = testFixture;
			testInfoData->Position.Test = test;
			testInfoData->MetadataId = test->GetMetadataId();

			if (dataRow == 0)
			{
				testInfoData->Position.DataRow = 0;
				testInfoData->Name = test->GetName();
				
				if (test->GetDataSource() != 0)
				{
					testInfoData->Kind = KindGroup;
					position->DataRow = test->GetDataSource()->GetHead();
				}
				else
				{
					testInfoData->Kind = KindTest;
					position->Test = test->GetNext();
					if (position->Test == 0)
						position->TestFixture = testFixture->GetNext();
				}

				return 1;
			}
		
			testInfoData->Kind = KindRowTest;
			testInfoData->Position.DataRow = dataRow;
			testInfoData->Name = test->GetDataSource()->GetDataRowDescription(dataRow).GetBuffer();
			position->DataRow = test->GetDataSource()->GetNextRow(dataRow);
			if (position->DataRow == 0)
				position->Test = test->GetNext();
            if (position->Test == 0)
                position->TestFixture = testFixture->GetNext();
            return 1;
        }

        void __cdecl MbUnitCpp_RunTest(Position* position, TestResultData* testResultData)
        {
            Test* test = position->Test;

			try
			{
				test->Run(testResultData, position->DataRow);
			}
			catch (...)
			{
			}
        }

		wchar_t* __cdecl MbUnitCpp_GetString(StringId stringId)
		{
			StringMap& map = TestFixture::GetStringMap();
			return map.Get(stringId)->GetBuffer();
		}

		void __cdecl MbUnitCpp_ReleaseString(StringId stringId)
		{
			StringMap& map = TestFixture::GetStringMap();
			map.Remove(stringId);
		}

		void __cdecl MbUnitCpp_ReleaseAllStrings()
		{
			StringMap& map = TestFixture::GetStringMap();
			map.RemoveAll();
		}
    }
}

#if defined(_WIN64) 
#pragma comment(linker, "/EXPORT:MbUnitCpp_GetHeadTest") 
#pragma comment(linker, "/EXPORT:MbUnitCpp_GetNextTest") 
#pragma comment(linker, "/EXPORT:MbUnitCpp_RunTest") 
#pragma comment(linker, "/EXPORT:MbUnitCpp_GetString") 
#pragma comment(linker, "/EXPORT:MbUnitCpp_ReleaseString") 
#pragma comment(linker, "/EXPORT:MbUnitCpp_ReleaseAllStrings") 
#else 
#pragma comment(linker, "/EXPORT:MbUnitCpp_GetHeadTest=_MbUnitCpp_GetHeadTest") 
#pragma comment(linker, "/EXPORT:MbUnitCpp_GetNextTest=_MbUnitCpp_GetNextTest") 
#pragma comment(linker, "/EXPORT:MbUnitCpp_RunTest=_MbUnitCpp_RunTest") 
#pragma comment(linker, "/EXPORT:MbUnitCpp_GetString=_MbUnitCpp_GetString") 
#pragma comment(linker, "/EXPORT:MbUnitCpp_ReleaseString=_MbUnitCpp_ReleaseString") 
#pragma comment(linker, "/EXPORT:MbUnitCpp_ReleaseAllStrings=_MbUnitCpp_ReleaseAllStrings") 
#endif
