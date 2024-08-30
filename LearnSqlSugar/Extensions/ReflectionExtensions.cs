using LearnSqlSugar.Models;
using SqlSugar;
using System.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearnSqlSugar.Extensions
{
    public static class ReflectionExtensions
    {
        /// <summary>
        /// 检查 obj中是否存在某个特性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static bool IsContainsAttribute<T>(object obj) where T : Attribute
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));

            Type type = obj.GetType();
            bool hasAttribute = Attribute.IsDefined(type, typeof(T));
            return hasAttribute;
        }

        /// <summary>
        /// 查找指定程序集文件中所有具有指定特性的类型。
        /// </summary>
        /// <typeparam name="TAttribute">要查找的特性类型。</typeparam>
        /// <param name="dllPath">程序集的文件路径。</param>
        /// <returns>所有具有指定特性的类型列表。</returns>
        public static List<Type> GetTypesWithAttributeByPath<TAttribute>(string dllPath) where TAttribute : Attribute
        {
            if (string.IsNullOrWhiteSpace(dllPath))
            {
                throw new ArgumentException("DLL 文件路径不能为空或空白。", nameof(dllPath));
            }

            if (!File.Exists(dllPath))
            {
                throw new FileNotFoundException("找不到指定的 DLL 文件。", dllPath);
            }

            // 加载程序集
            Assembly assembly = Assembly.LoadFrom(dllPath);

            // 查找具有特性的类型
            var typesWithAttribute = assembly.GetTypes()
                                             .Where(t => t.GetCustomAttributes(typeof(TAttribute), false).Any())
                                             .ToList();

            return typesWithAttribute;
        }

        public static List<Type> GetTypesWithAttribute<TAttribute>(string assemblyName) where TAttribute : Attribute
        {
            if (string.IsNullOrWhiteSpace(assemblyName))
            {
                throw new ArgumentException("程序集名称不能为空或空白。", nameof(assemblyName));
            }

            try
            {
                // 加载程序集
                Assembly assembly = Assembly.Load(assemblyName);

                // 查找具有特性的类型
                //特性继承: GetCustomAttributes 方法的第二个参数是 inherit，设置为 false 表示只查找直接应用于类型的特性，而不是从基类继承的特性。
                var typesWithAttribute = assembly.GetTypes()
                                                 .Where(t => t.GetCustomAttributes(typeof(TAttribute), false).Any())
                                                 .ToList();

                return typesWithAttribute;
            }
            catch (FileNotFoundException)
            {
                throw new FileNotFoundException("找不到指定的程序集。", assemblyName);
            }
            catch (BadImageFormatException)
            {
                throw new BadImageFormatException("程序集无效或不兼容。");
            }
            catch (Exception ex)
            {
                throw new Exception("加载程序集时发生错误: " + ex.Message, ex);
            }
        }

    }
}
