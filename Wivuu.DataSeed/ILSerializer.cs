using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

namespace Wivuu.DataSeed
{
    public static class ILSerializer
    {
        #region Fields

        private static readonly ModuleBuilder _assemblyModule;

        #endregion

        #region Constructor

        static ILSerializer()
        {
            _assemblyModule = CreateModule();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Compile expression to type
        /// </summary>
        public static T Compile<T>(Expression<T> expression)
            where T : class
        {
            var typeBuilder = _assemblyModule.DefineType(
                $"{expression.Name}op{Guid.NewGuid().ToString("N")}");

            var methodBuilder = typeBuilder.DefineMethod(
                "Operation", MethodAttributes.Public | MethodAttributes.Static);

            expression.CompileToMethod(methodBuilder);

            return Delegate.CreateDelegate(
                expression.Type,
                typeBuilder.CreateType().GetMethod("Operation")
            ) as T;
        }

        static ModuleBuilder CreateModule() =>
            // Define dynamic assembly
            AppDomain.CurrentDomain.DefineDynamicAssembly(
                new AssemblyName(nameof(ILSerializer) + Guid.NewGuid().ToString("N")),
                AssemblyBuilderAccess.RunAndSave
            ).DefineDynamicModule("Module");

        static Delegate CompileToType(
            this ModuleBuilder module,
            Type type, LambdaExpression expression)
        {
            var typeBuilder = module.DefineType(
                type.Name + "Operation" + Guid.NewGuid().ToString("N"));

            var methodBuilder = typeBuilder.DefineMethod(
                "Operation", MethodAttributes.Public | MethodAttributes.Static);

            expression.CompileToMethod(methodBuilder);

            return Delegate.CreateDelegate(
                expression.Type,
                typeBuilder.CreateType().GetMethod("Operation")
            );
        }

        #endregion
    }
}
