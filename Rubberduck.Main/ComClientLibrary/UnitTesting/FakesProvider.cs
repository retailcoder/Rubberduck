using Rubberduck.Resources.Registration;
using Rubberduck.UnitTesting.Fakes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Rubberduck.UnitTesting
{
    [
        ComVisible(true),
        Guid(RubberduckGuid.FakesProviderClassGuid),
        ProgId(RubberduckProgId.FakesProviderProgId),
        ClassInterface(ClassInterfaceType.None),
        ComDefaultInterface(typeof(IFakesProvider)),
        EditorBrowsable(EditorBrowsableState.Always)
    ]
    public class FakesProvider : IFakesProvider, IFakes
    // IFakesProvider is COM side, exposed to the VBA User
    // IFakes is Rubberduck side and we inject the FakesProvider back into Core
    {
        internal const int AllInvocations = -1;
        // ReSharper disable once InconsistentNaming - respects COM naming conventions
        [Description("A value indicating that specified configuration applies to all invocations.")]
        public const int rdAllInvocations = AllInvocations;

        private static Dictionary<Type, StubBase> ActiveFakes { get; } = new Dictionary<Type, StubBase>();

        internal bool CodeIsUnderTest { get; set; }

        public void StartTest()
        {
            if (CodeIsUnderTest)
            {
                return;
            }
            CodeIsUnderTest = true;
        }

        public static void SuspendFake(Type type)
        {
            foreach (var fake in ActiveFakes.Values)
            {
                if (fake.GetType() == type)
                {
                    fake.DisableHook();
                    return;
                }
            }
        }

        public static void ResumeFake(Type type)
        {
            foreach (var fake in ActiveFakes.Values)
            {
                if (fake.GetType() == type)
                {
                    fake.EnableHook();
                    return;
                }
            }
        }

        public void StopTest()
        {
            foreach (var fake in ActiveFakes.Values)
            {
                fake.Dispose();
            }
            ActiveFakes.Clear();
            CodeIsUnderTest = false;
        }

        public void StartHeadlessTest()
        {
            if (CodeIsUnderTest)
            {
                return;
            }

            var headlessFakes = new object[]
            {
                CreateThrowingActiveFake<MsgBox>(),
                CreateThrowingActiveFake<InputBox>(),
                CreateThrowingActiveFake<SendKeys>(),
                CreateThrowingActiveFake<Shell>(),
                CreateThrowingActiveFake<DeleteSetting>(),
                CreateThrowingActiveFake<SaveSetting>(),
                CreateThrowingActiveFake<GetSetting>(),
                CreateThrowingActiveFake<GetAllSettings>(),
                CreateThrowingActiveFake<Kill>(),
                CreateThrowingActiveFake<Dir>(),
                CreateThrowingActiveFake<MkDir>(),
                CreateThrowingActiveFake<RmDir>(),
                CreateThrowingActiveFake<ChDir>(),
                CreateThrowingActiveFake<ChDrive>(),
                CreateThrowingActiveFake<CurDir>(),
                CreateThrowingActiveFake<FreeFile>(),
                CreateThrowingActiveFake<SetAttr>(),
                CreateThrowingActiveFake<GetAttr>(),
                CreateThrowingActiveFake<FileLen>(),
                CreateThrowingActiveFake<FileDateTime>(),
                CreateThrowingActiveFake<IMEStatus>(),
                CreateThrowingActiveFake<FileCopy>(),
            };

            CodeIsUnderTest = true;
        }

        private T CreateThrowingActiveFake<T>()
            where T : StubBase, new()
        {
            var fake = RetrieveOrCreateFunction<T>();
            fake.RaisesError(19997, $"Intercepted invocation: '{typeof(T).Name}' fake was not configured for headless run.");

            return fake;
        }

        private T RetrieveOrCreateFunction<T>()
            where T : StubBase, new()
        {
            return RetrieveOrCreateFunction(() => new T());
        }

        private T RetrieveOrCreateFunction<T>(Func<T> factory)
            where T : StubBase
        {
            var type = typeof(T);

            CodeIsUnderTest = true;
            if (!ActiveFakes.ContainsKey(type))
            {
                ActiveFakes.Add(type, factory.Invoke());
            }

            var fake = ActiveFakes[type] as T;
            return fake;
        }

        #region Function Overrides

        public IFake MsgBox => RetrieveOrCreateFunction<MsgBox>();
        public IFake InputBox => RetrieveOrCreateFunction<InputBox>();
        public IStub Beep => RetrieveOrCreateFunction(() => new Beep(VbeProvider.BeepInterceptor));
        public IFake Environ => RetrieveOrCreateFunction<Environ>();
        public IFake Timer => RetrieveOrCreateFunction<Timer>();
        public IFake DoEvents => RetrieveOrCreateFunction<DoEvents>();
        public IFake Shell => RetrieveOrCreateFunction<Shell>();
        public IStub SendKeys => RetrieveOrCreateFunction<SendKeys>();
        public IStub Kill => RetrieveOrCreateFunction<Kill>();
        public IStub MkDir => RetrieveOrCreateFunction<MkDir>();
        public IStub RmDir => RetrieveOrCreateFunction<RmDir>();
        public IStub ChDir => RetrieveOrCreateFunction<ChDir>();
        public IStub ChDrive => RetrieveOrCreateFunction<ChDrive>();
        public IFake CurDir => RetrieveOrCreateFunction<CurDir>();
        public IFake Now => RetrieveOrCreateFunction<Now>();
        public IFake Time => RetrieveOrCreateFunction<Time>();
        public IFake Date => RetrieveOrCreateFunction<Date>();
        public IFake Rnd => RetrieveOrCreateFunction<Rnd>();
        public IStub DeleteSetting => RetrieveOrCreateFunction<DeleteSetting>();
        public IStub SaveSetting => RetrieveOrCreateFunction<SaveSetting>();
        public IFake GetSetting => RetrieveOrCreateFunction<GetSetting>();
        public IStub Randomize => RetrieveOrCreateFunction<Randomize>();
        public IFake GetAllSettings => RetrieveOrCreateFunction<GetAllSettings>();
        public IStub SetAttr => RetrieveOrCreateFunction<SetAttr>();
        public IFake GetAttr => RetrieveOrCreateFunction<GetAttr>();
        public IFake FileLen => RetrieveOrCreateFunction<FileLen>();
        public IFake FileDateTime => RetrieveOrCreateFunction<FileDateTime>();
        public IFake FreeFile => RetrieveOrCreateFunction<FreeFile>();
        public IFake IMEStatus => RetrieveOrCreateFunction<IMEStatus>();
        public IFake Dir => RetrieveOrCreateFunction<Dir>();
        public IStub FileCopy => RetrieveOrCreateFunction<FileCopy>();
        #endregion

        public IParams Params { get; } = new Params();
    }
}
