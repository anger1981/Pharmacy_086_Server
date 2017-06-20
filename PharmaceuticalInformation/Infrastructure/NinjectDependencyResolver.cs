using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Test_pharm_server.PharmaceuticalInformation.DataTools;
using Test_pharm_server.PharmaceuticalInformation.Interfaces;

namespace Test_pharm_server.PharmaceuticalInformation.Infrastructure
{
    public static class NinjectDependencyResolver
    {
        public static StandardKernel kernel = new StandardKernel();

        //static NinjectDependencyResolver()
        //{
        //    kernel
        //        .Bind<IPharmacyInformation>()
        //        .To<EFPharmacyInformationRepository>()
        //        .InSingletonScope()
        //        .WithConstructorArgument("ConnectionString", ConnectionString);
        //}

        public static void AddBindings(string ConnectionString)
        {
            kernel
                .Bind<IPharmacyInformation>()
                .To<EFPharmacyInformationRepository>()
                .InSingletonScope()
                .WithConstructorArgument("ConnectionString", ConnectionString);
        }
    }
}
