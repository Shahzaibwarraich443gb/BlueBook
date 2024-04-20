using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using AccountingBlueBook.AppServices.JobTitles.Dto;
using AccountingBlueBook.AppServices.JobTitles;
using AccountingBlueBook.Entities.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.ObjectMapping;
using AccountingBlueBook.Entities.MainEntities;
using AccountingBlueBook.AppServices.Venders.Dto;
using Microsoft.EntityFrameworkCore;
using AccountingBlueBook.AppServices.Customers.Dto;
using AccountingBlueBook.AppServices.VendorContactInfos.Dto;
using System.ComponentModel.Design;
using AccountingBlueBook.AppServices.Invoices.dto;
using AccountingBlueBook.Authorization.Users.Dto;
using System.IO;
using MimeKit;
using AccountingBlueBook.Authorization.Users;

namespace AccountingBlueBook.AppServices.Venders
{
    public class VenderAppService : AccountingBlueBookAppServiceBase, IVenderAppService
    {
        private readonly IRepository<Vendor> _vendorRepository;
        private readonly IRepository<Company> _companyRepository;
        private readonly IRepository<VendorContactInfo> _vendorContactInfoRepository;
        private readonly IRepository<VendorAddress> _vendorAddressesRepository;
        private readonly IEmailAppServices _emailAppService;
        private IObjectMapper ObjectMapper;

        public VenderAppService(IRepository<Vendor> vendorRepository, IRepository<Company> companyRepository, IObjectMapper objectMapper,
            IRepository<VendorContactInfo> vendorContactInfoRepository, IRepository<VendorAddress> vendorAddressesRepository, IEmailAppServices emailAppService)
        {
            _vendorRepository = vendorRepository;
            _companyRepository = companyRepository;
            ObjectMapper = objectMapper;
            _vendorContactInfoRepository = vendorContactInfoRepository;
            _vendorAddressesRepository = vendorAddressesRepository;
            _emailAppService = emailAppService;
        }

        public async Task<long> CreateOrEdit(CreateOrEditVenderDto input)
        {
            long vendorId;

            if (input.Id > 0)
                vendorId = await Update(input);
            else
                vendorId = await Cearte(input);

            return vendorId;
        }


        [UnitOfWork]
        private async Task<long> Update(CreateOrEditVenderDto input)
        {
            var vendor = await _vendorRepository.FirstOrDefaultAsync((int)input.Id);
            vendor.IsActive = input.IsActive;
            vendor.BusinessName = input.BusinessName;
            vendor.SSN = input.SSN;
            vendor.TaxId = input.TaxId;
            vendor.DateOfBirth = input.DateOfBirth;
            vendor.VenderTypeId = input.VenderTypeId;
            vendor.VendorName = input.VendorName;
            vendor.Description = input.Description;
            await _vendorRepository.UpdateAsync(vendor);
            return input.Id;
        }

        [UnitOfWork]
        private async Task<long> Cearte(CreateOrEditVenderDto input)
        {
            var vender = ObjectMapper.Map<Vendor>(input);
            var company = await _companyRepository.FirstOrDefaultAsync(x => x.TenantId == (int)AbpSession.TenantId);
            vender.CompanyId = company.Id;
            vender.IsActive = true;
            long vendorId = await _vendorRepository.InsertOrUpdateAndGetIdAsync(vender);
            await CurrentUnitOfWork.SaveChangesAsync();
            return vendorId;
        }

        public async Task Delete(EntityDto input)
        {
            var jobTitle = await _vendorRepository.FirstOrDefaultAsync((int)input.Id);
            await _vendorRepository.DeleteAsync(jobTitle);
            await CurrentUnitOfWork.SaveChangesAsync();
        }
        public async Task<CreateOrEditVenderDto> GetVendor(EntityDto input)
        {
            var vendorDto = await _vendorRepository.GetAll().FirstOrDefaultAsync(x => x.Id == input.Id);
            if (vendorDto != null)
            {
                var obj = new CreateOrEditVenderDto();
                obj.Id = vendorDto.Id;
                obj.VenderTypeId = vendorDto.VenderTypeId != null ? vendorDto.VenderTypeId : null;
                obj.BusinessName = vendorDto.BusinessName != null ? vendorDto.BusinessName : null;
                obj.VendorName = vendorDto.VendorName != null ? vendorDto.VendorName : null;
                obj.Description = vendorDto.Description != null ? vendorDto.Description : null;
                obj.DateOfBirth = vendorDto.DateOfBirth != null ? vendorDto.DateOfBirth : System.DateTime.Now;
                obj.IsActive = vendorDto.IsActive;
                obj.SSN = vendorDto.SSN != null ? vendorDto.SSN : null;
                obj.TaxId = vendorDto.TaxId != null ? vendorDto.TaxId : 0;
                obj.CompanyId = vendorDto.CompanyId;
                return obj;
            }
            return null;
        }

        public async Task<List<VendorDto>> GetAll()
        {
            var company = await _companyRepository.FirstOrDefaultAsync(x => x.TenantId == (int)AbpSession.TenantId);
            var filteredQuery = _vendorRepository.GetAll().Where(x => x.CompanyId == company.Id).AsQueryable();

            var data = from o in filteredQuery
                       select new VendorDto
                       {
                           Id = o.Id,
                           VendorName = o.VendorName,
                           Description = o.Description,
                           Email = _vendorContactInfoRepository.GetAll().Where(a => a.VendorId == o.Id).Select(a => a.EmailAddress).FirstOrDefault(),
                           PhoneNumber = _vendorContactInfoRepository.GetAll().Where(a => a.VendorId == o.Id).Select(a => a.PhoneNumber).FirstOrDefault(),
                           SSN = o.SSN,
                           TaxId = o.TaxId,
                           BusinessName = o.BusinessName,
                           DateOfBirth = o.DateOfBirth,
                           VenderTypeId = o.VenderTypeId,
                           CompanyName = company.Name,
                           IsActive = o.IsActive,
                       };
            return await data.ToListAsync();
        }

        public async Task SendEmailToVendor(string Subject, string EmailBody, string FileName, int VendorId)
        {
            var TenantId = AbpSession.TenantId;
            string subject = Subject;
            StringBuilder emailbody = new StringBuilder();
            emailbody.Append(EmailBody);

            var folderName = Path.Combine("wwwroot", "Files", "VendorAttachments");
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), folderName);

            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = emailbody.ToString();

            var attachmentStreams = new List<Tuple<string, MemoryStream>>(); // Move this outside the loop

            if (Directory.Exists(folderPath))
            {
                var fileNames = FileName.Split(',');
                foreach (var fileName in fileNames)
                {
                    var filePath = Path.Combine(folderPath, fileName);
                    if (File.Exists(filePath))
                    {
                        using (var stream = new FileStream(filePath, FileMode.Open))
                        {
                            var memoryStream = new MemoryStream();
                            await stream.CopyToAsync(memoryStream);

                            memoryStream.Position = 0;
                            bodyBuilder.Attachments.Add(fileName, memoryStream);
                            attachmentStreams.Add(new Tuple<string, MemoryStream>(fileName, memoryStream));
                        }
                    }
                }
            }

            await _emailAppService.SendMail(new EmailsDto()
            {
                Subject = subject,
                Body = emailbody.ToString(),
                ToEmail = _vendorContactInfoRepository.GetAll().Where(a => a.VendorId == VendorId).Select(a => a.EmailAddress).FirstOrDefault(),
                Streams = attachmentStreams
            });
        }

    }
}

