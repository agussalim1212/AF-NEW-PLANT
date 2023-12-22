using MediatR;
using SkeletonApi.Shared;
using AutoMapper;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Domain.Entities;

namespace SkeletonApi.Application.Features.Accounts.Profiles.Commands.CreateAccount
{
    internal class CreateAccountCommandHandler : IRequestHandler<CreateAccountRequest, Result<CreateAccountResponseDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAccountRepository _accountRepository;

        public CreateAccountCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IAccountRepository accountRepository)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _accountRepository = accountRepository;
        }

        public async Task<Result<CreateAccountResponseDto>> Handle(CreateAccountRequest request, CancellationToken cancellationToken)
        {
            var account = _mapper.Map<Account>(request);

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" }; // ekstensi yang diizinkan
            var fileExtension = Path.GetExtension(request.img_path.FileName).ToLower();

            if (!allowedExtensions.Contains(fileExtension))
            {
                return await Result<CreateAccountResponseDto>.FailureAsync("Invalid file extension. Only JPG, JPEG, and PNG are allowed.");
            }

            // Membatasi ukuran berkas
            var maxFileSizeInBytes = 2 * 1024 * 1024; // ukuran maksimum (dalam byte)

            if (request.img_path.Length > maxFileSizeInBytes)
            {
                return await Result<CreateAccountResponseDto>.FailureAsync("File size exceeds the maximum allowed size (2 MB).");
            }

            var path = Path.Combine("wwwroot/FotoPath/", request.img_path.FileName);

            using (FileStream stream = new FileStream(path, FileMode.Create))
            {
                await request.img_path.CopyToAsync(stream);
                stream.Close();
            }

            var cekAccount = await _accountRepository.ValidateAccount(account);
            if (cekAccount == false)
            {
                var cek = _unitOfWork.Repository<Account>().FindByCondition(a => a.Username == account.Username).FirstOrDefault();
                cek.PhotoURL = request.img_path.FileName;
                cek.CreatedAt = DateTime.UtcNow;
                cek.UpdatedAt = DateTime.UtcNow;
                await _unitOfWork.Repository<Account>().UpdateAsync(cek);
                await _unitOfWork.Save(cancellationToken);

            }
            else
            {
                account.PhotoURL = request.img_path.FileName;
                account.CreatedAt = DateTime.UtcNow;
                account.UpdatedAt = DateTime.UtcNow;
                await _unitOfWork.Repository<Account>().AddAsync(account);
                account.AddDomainEvent(new AccountCreatedEvent(account));
                await _unitOfWork.Save(cancellationToken);
            }

            var accountResponse = _mapper.Map<CreateAccountResponseDto>(account);
            return await Result<CreateAccountResponseDto>.SuccessAsync(accountResponse, "Created.");
        }
    }
}
