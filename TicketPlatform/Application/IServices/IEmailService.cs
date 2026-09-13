using System;
using System.Collections.Generic;
using System.Text;

namespace Application.IServices
{
    public interface IEmailService
    {
        Task SendConfirmEmailAsync(string to, string confirmationLink, CancellationToken cancellationToken);
        Task SendResetPasswordEmailAsync(string to, string confirmationLink, CancellationToken cancellationToken);

    }

}
