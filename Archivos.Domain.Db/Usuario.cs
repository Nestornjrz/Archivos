//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Archivos.Domain.Db
{
    using System;
    using System.Collections.Generic;
    
    public partial class Usuario
    {
        public Usuario()
        {
            this.ArchivosMovimientos = new HashSet<ArchivosMovimiento>();
        }
    
        public int UsuarioID { get; set; }
        public string NombreUsuario { get; set; }
        public System.Guid UserID { get; set; }
        public string CorreoElectronico { get; set; }
    
        public virtual ICollection<ArchivosMovimiento> ArchivosMovimientos { get; set; }
    }
}
