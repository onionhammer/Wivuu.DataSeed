using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

namespace Wivuu.DataSeed
{
    public static class ILSerializer
    {
        #region Fields

        private static readonly ModuleBuilder _assemblyModule = CreateModule();

        #endregion

        #region Methods

        static string StringId =>
            Guid.NewGuid().ToString("N").Substring(0, 6);

        static ModuleBuilder CreateModule() =>
            // Define dynamic assembly
            AppDomain.CurrentDomain.DefineDynamicAssembly(
                new AssemblyName(nameof(ILSerializer) + StringId),
                AssemblyBuilderAccess.RunAndSave
            ).DefineDynamicModule("Module");

        /// <summary>
        /// Compile expression to type
        /// </summary>
        public static T Compile<T>(Expression<T> expression)
            where T : class
        {
            var typeBuilder = _assemblyModule.DefineType(
                $"{typeof(T).Name}op{StringId}");

            MethodBuilder operation;
            operation = typeBuilder.DefineMethod(nameof(operation), 
                MethodAttributes.Public | MethodAttributes.Static);

            expression.CompileToMethod(operation);

            return Delegate.CreateDelegate(
                expression.Type,
                typeBuilder.CreateType().GetMethod(nameof(operation))
            ) as T;
        }

        #endregion
    }
}