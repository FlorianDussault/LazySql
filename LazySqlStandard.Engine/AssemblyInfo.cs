using System.Runtime.CompilerServices;

#if DEBUG || APPVEYOR
[assembly: InternalsVisibleTo("LazySqlCore.UnitTest")]
#endif