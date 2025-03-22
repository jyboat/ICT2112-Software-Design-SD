using ClearCare.Interfaces;
using ClearCare.Models;
using ClearCare.Gateways;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClearCare.Observer;

namespace ClearCare.Controls
{
    public class DrugInteractionControl
    {
        private readonly PatientDrugMapper _mapper;

        //Mapper
        public DrugInteractionControl(PatientDrugMapper mapper)
        {
            _mapper = mapper;
        }
    }
}
