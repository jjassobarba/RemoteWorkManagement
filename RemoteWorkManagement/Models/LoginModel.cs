using System.ComponentModel.DataAnnotations;

namespace RemoteWorkManagement.Models
{
    public sealed class LoginModel
    {
        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        /// <value>
        /// The email.
        /// </value>
        [Required]
        [StringLength(50, ErrorMessage = "El email debe ser menor de 50 y mayor 4 caracteres .", MinimumLength = 5)]
        [RegularExpression(@"^.*@.*\..*$", ErrorMessage = "El email no es valido.")]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>
        /// The password.
        /// </value>
        [Required]
        [StringLength(250, ErrorMessage = "La contraseña debe ser menor de 250 y mayor a 6 caracteres.", MinimumLength = 6)]
        [RegularExpression(@"^(?=.*\d)(?=.*[a-zA-Z]).{6,250}$", ErrorMessage = "La contraseña debe tener letras y numeros.")]
        public string Password { get; set; }
    }
}