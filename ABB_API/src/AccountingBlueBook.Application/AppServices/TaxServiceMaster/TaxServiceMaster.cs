using AccountingBlueBook.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.AppServices.TaxServiceMaster
{
    public class TaxServiceMaster : AccountingBlueBookAppServiceBase, ITaxServiceMaster
    {
        public async Task<List<TaxFillingStatusDto>> GetTaxFillingStatus()
        {
            List<TaxFillingStatusDto> statusList = new List<TaxFillingStatusDto>();

            foreach (TaxFillingStatusEnum statusEnum in Enum.GetValues(typeof(TaxFillingStatusEnum)))
            {
                var status = new TaxFillingStatusDto
                {
                    Value = (int)statusEnum,
                    Name = statusEnum.ToString()
                };
                statusList.Add(status);
            }
            return statusList;
        }

        public async Task<List<TenureDto>> GetTenure()
        {
            List<TenureDto> tenureList = new List<TenureDto>();

            foreach (TenureEnum statusEnum in Enum.GetValues(typeof(TenureEnum)))
            {
                var tenures = new TenureDto
                {
                    Value = (int)statusEnum,
                    Name = statusEnum.ToString()
                };
                tenureList.Add(tenures);
            }
            return tenureList;
        }

        public async Task<List<FormEnum>> GetFormList()
        {
            List<FormEnum> formList = Enum.GetValues(typeof(FormEnum)).Cast<FormEnum>().ToList();
            return formList;
        }

        private string GetEnumMemberValue(Enum enumValue)
        {
            var memberInfo = enumValue.GetType().GetMember(enumValue.ToString()).FirstOrDefault();
            if (memberInfo != null)
            {
                var attribute = memberInfo.GetCustomAttributes(typeof(EnumMemberAttribute), false).FirstOrDefault() as EnumMemberAttribute;
                if (attribute != null)
                {
                    return attribute.Value;
                }
            }
            return enumValue.ToString();
        }

        public async Task<List<string>> GetIncomeDescriptions()
        {
            List<IncomeDescriptionEnum> incomeDescList = Enum.GetValues(typeof(IncomeDescriptionEnum)).Cast<IncomeDescriptionEnum>().ToList();
            List<string> incomeDescStrings = incomeDescList.Select(e => GetEnumMemberValue(e)).ToList();
            return incomeDescStrings;
        }

    }
}
