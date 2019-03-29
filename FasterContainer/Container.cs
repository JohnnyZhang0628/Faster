using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FasterContainer
{

    public class Container
    {
        private static Dictionary<string, Type> cacheDic = new Dictionary<string, Type>();
        /// <summary>
        /// 注册类型
        /// </summary>
        /// <typeparam name="IT">抽象类</typeparam>
        /// <typeparam name="T">抽象实现类</typeparam>
        public void RegisterType<IT, T>()
        {
            //设置缓存
            cacheDic.Add(typeof(IT).FullName, typeof(T));
        }

        /// <summary>
        /// 创建类型
        /// </summary>
        /// <typeparam name="IT"></typeparam>
        /// <returns></returns>
        public IT Resolve<IT>()
        {
            string key = typeof(IT).FullName;
            Type type = (Type)cacheDic[key];
            object oValue = Create(type);
            return (IT)oValue;
        }

        private object Create(Type type)
        {
            //优先标记特性，就找参数个数最多的
            var ctorArray = type.GetConstructors();
            ConstructorInfo ctor = null;
            if (ctorArray.Where(c => c.IsDefined(typeof(InjectionConstructorAttribute), true)).Count() > 0)
            {
                ctor = ctorArray.Where(c => c.IsDefined(typeof(InjectionConstructorAttribute), true)).FirstOrDefault();
            }
            else
            {
                ctor = ctorArray.OrderByDescending(c => c.GetParameters().Length).FirstOrDefault();
            }
            var paraArray = ctor.GetParameters();
            if (paraArray.Length == 0)
            {
                return Activator.CreateInstance(type);
            }
            List<object> listPara = new List<object>();
            foreach (var para in paraArray)
            {
                string keyType = para.ParameterType.FullName;
                if (cacheDic.ContainsKey(keyType))
                {
                    object oPara = Create(cacheDic[keyType]);//这里递归的
                    listPara.Add(oPara);
                }
                else
                    throw new Exception($"please first register {keyType} type");
            }
            return Activator.CreateInstance(type, listPara.ToArray());
        }
    }
}
