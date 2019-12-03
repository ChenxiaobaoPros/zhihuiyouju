using System;
using System.Collections.Generic;
using Autofac;
using Autofac.Configuration;
using Autofac.Configuration.Elements;

using ICSharpCode.SharpZipLib.Zip;

namespace Aoto.PPS.Infrastructure
{
    public sealed class AutofacContainer
    {
        private static IContainer container;
        private static IDictionary<string, Type> mameTypeMapper;

        static AutofacContainer()
        {
            mameTypeMapper = new Dictionary<string, Type>();
            ContainerBuilder builder = new ContainerBuilder();

           // builder.RegisterInstance<ISqlMapper>().SingleInstance();
            //builder.RegisterInstance<ISqlMapper>(new DomSqlMapBuilder().Configure("config\\SqlMap.config")).SingleInstance();
            builder.RegisterInstance<FastZip>(new FastZip()).SingleInstance();
            
            //DomSqlMapBuilder sqlMapbuilder = new DomSqlMapBuilder();
            //ISqlMapper sqlMapper = sqlMapbuilder.Configure("config\\SqlMap.config");
            //builder.RegisterInstance<ISqlMapper>(sqlMapper).As<ISqlMapper>().SingleInstance();

            //builder.Register<ISqlMapper>(c => new DomSqlMapBuilder().Configure("config\\SqlMap.config")).SingleInstance();

            ConfigurationSettingsReader reader = new ConfigurationSettingsReader("autofac");
            builder.RegisterModule(reader);

            //builder.Register(c => new DomSqlMapBuilder() { c.});

            foreach (ComponentElement e in reader.SectionHandler.Components)
            {
                if (e.Service.Contains(","))
                {
                    mameTypeMapper.Add(e.Name, Type.GetType(e.Service));
                }
                else
                {
                    mameTypeMapper.Add(e.Name, reader.SectionHandler.DefaultAssembly.GetType(e.Service));
                }
            }

            //if (File.Exists(Path.Combine(Config.AppRoot, "Aoto.PPS.Extensions.dll")))
            //{
            //    Assembly a = Assembly.Load("Aoto.PPS.Extensions");
            //    Type baseType = typeof(IAutoActivated);
            //    builder.RegisterAssemblyTypes(a).Where(t => baseType.IsAssignableFrom(t) && t != baseType).AsImplementedInterfaces().SingleInstance().AutoActivate();
            //}
 
            container = builder.Build();

            //builder.Register(c => new A { B = c.Resolve<B>() });
            //为了提供循环依赖（就是当A使用B的时候B已经初始化），需要使用OnActivated事件接口：


            //builder.Register(c => new A()).OnActivated(e => e.Instance.B = e.Context.Resolve<B>());
            //通过发射，使用PropertiesAutowired（）修饰符注入属性。


            //builder.RegisterType<A>().PropertiesAutowired();
            //如果你预先知道属性的名字和值，你可以使用

            //builder.WithProperty("propertyName", propertyValue)。
        }

        public static object ResolveNamed(string name)
        {
            return container.ResolveNamed(name, mameTypeMapper[name]);
        }

        public static T ResolveNamed<T>(string name)
        {
            return container.ResolveNamed<T>(name);
        }

        public static T Resolve<T>()
        {
            return container.Resolve<T>();
        }
    }
}