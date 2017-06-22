using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerOfSystem.PharmaceuticalInformation.Interfaces;

namespace ServerOfSystem.PharmaceuticalInformation.DataTools
{
    class PharmacyInformationRelease
    {
        public IPharmacyInformation PhrmInf;

        public PharmacyInformationRelease(IPharmacyInformation _PhrmInf)
        {
            PhrmInf = _PhrmInf;
        }

    }
}
