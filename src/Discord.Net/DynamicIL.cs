using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Discord
{
    internal static class DynamicIL
    {
        public static Action<T, T> CreateCloner<T>()
        {
            var method = new DynamicMethod("CopyFields", null, new[] { typeof(T), typeof(T) }, typeof(T), true);
            var generator = method.GetILGenerator();
            var typeInfo = typeof(T).GetTypeInfo();
            
            CopyFields(generator, typeInfo);

            generator.Emit(OpCodes.Ret);

            return method.CreateDelegate(typeof(Action<T, T>)) as Action<T, T>;
        }
        private static void CopyFields(ILGenerator generator, TypeInfo typeInfo)
        {
            foreach (var field in typeInfo.DeclaredFields.Where(x => !x.IsStatic))
            {
                generator.Emit(OpCodes.Ldarg_1); //Stack: TargetRef
                generator.Emit(OpCodes.Ldarg_0); //Stack: TargetRef, SourceRef
                generator.Emit(OpCodes.Ldfld, field); //Stack: TargetRef, Value
                generator.Emit(OpCodes.Stfld, field); //Stack:
            }

            var baseType = typeInfo.BaseType;
            if (baseType != null && baseType.AssemblyQualifiedName == typeInfo.AssemblyQualifiedName)
                CopyFields(generator, baseType.GetTypeInfo());
        }
    }
}
