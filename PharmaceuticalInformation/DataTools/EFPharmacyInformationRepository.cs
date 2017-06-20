using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Test_pharm_server.PharmaceuticalInformation.Interfaces;

namespace Test_pharm_server.PharmaceuticalInformation.DataTools
{
    class EFPharmacyInformationRepository : IPharmacyInformation
    {
        private PhrmInfTESTEntities EFPharmacyInformation;

        public PhrmInfTESTEntities EFPhrmInf
        {
            get { return EFPharmacyInformation; }
        }

        public EFPharmacyInformationRepository(string ConnectionString)
        {
            EFPharmacyInformation = new PhrmInfTESTEntities(ConnectionString);
        }
    }
}
