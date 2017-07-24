using SistemaResocentro.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace SistemaResocentro.Member
{
    public class CustomMembershipProvider : MembershipProvider
    {
        public override string ApplicationName
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            throw new NotImplementedException();
        }

        public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
        {
            throw new NotImplementedException();
        }

        public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            throw new NotImplementedException();
        }

        public override bool EnablePasswordReset
        {
            get { throw new NotImplementedException(); }
        }

        public override bool EnablePasswordRetrieval
        {
            get { throw new NotImplementedException(); }
        }

        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override int GetNumberOfUsersOnline()
        {
            throw new NotImplementedException();
        }

        public override string GetPassword(string username, string answer)
        {
            throw new NotImplementedException();
        }

        public override MembershipUser GetUser(string username, bool userIsOnline)
        {

            if (string.IsNullOrEmpty(username))
            {
                // No user signed in
                return null;
            }
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                var u = (from x in db.USUARIO where x.codigousuario == username select x).SingleOrDefault();

                var _unidades = (from x in db.SUCURSALXUSUARIO
                                 join un in db.UNIDADNEGOCIO
                                 on x.codigounidad equals un.codigounidad
                                 where x.codigousuario == u.codigousuario
                                 select new
                                 {
                                     x.codigounidad,
                                     x.codigosucursal
                                 }).ToList();
                string[] uni = _unidades.Select(m => ((m.codigounidad * 100) + m.codigosucursal).ToString()).ToArray();

                var emp = db.EMPLEADO.Where(x => x.dni == u.dni).SingleOrDefault();

                CustomMembershipUser user = new CustomMembershipUser(
                           "MyMembershipProvider",
                         u.ShortName,//name
                         u.codigousuario,//providerUserKey
                         emp.email,//email
                         "",//passwordQuestion
                         emp.sexo,//comment
                         true,
                         false,
                         DateTime.MinValue,
                         DateTime.MinValue,
                         DateTime.MinValue,
                         DateTime.MinValue,
                         DateTime.MinValue);

                // Fill additional properties
                user.bloqueado = u.isSessionLocked;
                user.CustomerNumber = u.codigousuario;
                user.idcargo = emp.codigocargo.Value;
                user.sucursales = uni;
                user.clave = u.contrasena;
                user.sucursales_int = _unidades.Select(m => ((m.codigounidad * 100) + m.codigosucursal)).ToArray();
                user.siglas = u.siglas;
                user.telefono = emp.telefono;
                user.dni = emp.dni_corregido;
                var ruta = "\\\\192.168.0.5\\Perfiles\\" + u.codigousuario + ".png";

                user.pathBuddy = System.IO.File.Exists(ruta);


                return user;
            }
        }

        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            throw new NotImplementedException();
        }

        public override string GetUserNameByEmail(string email)
        {
            throw new NotImplementedException();
        }

        public override int MaxInvalidPasswordAttempts
        {
            get { throw new NotImplementedException(); }
        }

        public override int MinRequiredNonAlphanumericCharacters
        {
            get { throw new NotImplementedException(); }
        }

        public override int MinRequiredPasswordLength
        {
            get { throw new NotImplementedException(); }
        }

        public override int PasswordAttemptWindow
        {
            get { throw new NotImplementedException(); }
        }

        public override MembershipPasswordFormat PasswordFormat
        {
            get { throw new NotImplementedException(); }
        }

        public override string PasswordStrengthRegularExpression
        {
            get { throw new NotImplementedException(); }
        }

        public override bool RequiresQuestionAndAnswer
        {
            get { throw new NotImplementedException(); }
        }

        public override bool RequiresUniqueEmail
        {
            get { throw new NotImplementedException(); }
        }

        public override string ResetPassword(string username, string answer)
        {
            throw new NotImplementedException();
        }

        public override bool UnlockUser(string userName)
        {
            throw new NotImplementedException();
        }

        public override void UpdateUser(MembershipUser user)
        {
            throw new NotImplementedException();
        }

        public override bool ValidateUser(string username, string password)
        {
            using (DATABASEGENERALEntities dt = new DATABASEGENERALEntities())
            {
                USUARIO usu = (from x in dt.USUARIO
                             where x.siglas == username && x.contrasena == password && x.bloqueado==false
                             select x).FirstOrDefault();

                return (usu != null);
            }
        }
    }

    public class CustomMembershipUser : MembershipUser
    {
        public CustomMembershipUser(
              string providerName,
            string name,
            object providerUserKey,
            string email,
            string passwordQuestion,
                      string comment,
            bool isApproved,
            bool isLockedOut,
            DateTime creationDate,
            DateTime lastLoginDate,
                DateTime lastActivityDate,
                DateTime lastPasswordChangedDate,
            DateTime lastLockoutDate
            )
            : base(providerName, name, providerUserKey, email, passwordQuestion,
            comment, isApproved, isLockedOut, creationDate, lastLoginDate,
            lastActivityDate, lastPasswordChangedDate, lastLockoutDate)
        {
        }

        public string CustomerNumber { get; set; }
        public string[] sucursales { get; set; }
        public int[] sucursales_int { get; set; }
        public string siglas { get; set; }
        public string clave { get; set; }
        public bool bloqueado{ get; set; }

        public bool pathBuddy { get; set; }

        public string telefono { get; set; }
        public int idcargo{ get; set; }

        public string dni { get; set; }
    }
}