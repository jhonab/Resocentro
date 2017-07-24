using Resocentro_Desktop.Entitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Resocentro_Desktop.Complemento
{
    class SedacionStyle : StyleSelector
    {
        public Style sedacion { get; set; }
        public Style nsedacion { get; set; }
        public override Style SelectStyle(object item, System.Windows.DependencyObject container)
        {
            var entidad = item as VisorCita;
            if (entidad.sedacion)
                return this.sedacion;
            else
                return this.nsedacion;
        }
    }
    class SeleccionCita : StyleSelector
    {
        public override Style SelectStyle(object item, System.Windows.DependencyObject container)
        {
            var entidad = item as HCCita;
            if (entidad.isSeleccionado)
                return this.seleccionado;
            else
                return this.nseleccionado;

        }      
        public Style seleccionado { get; set; }
        public Style nseleccionado { get; set; }
    }
    class SeleccionAdmision : StyleSelector
    {
        public Style seleccionado { get; set; }
        public Style nseleccionado { get; set; }
        public override Style SelectStyle(object item, System.Windows.DependencyObject container)
        {
            var entidad = item as HCAdmision;
            if (entidad.isSeleccionado)
                return this.seleccionado;
            else
                return this.nseleccionado;
        }




    }
    class SeleccionPago : StyleSelector
    {
        public Style seleccionado { get; set; }
        public Style nseleccionado { get; set; }
        public override Style SelectStyle(object item, System.Windows.DependencyObject container)
        {
            var entidad = item as HCPagos;
            if (entidad.isSeleccionado)
                return this.seleccionado;
            else
                return this.nseleccionado;
        }
    }
    class SeleccionAdquision : StyleSelector
    {
        public Style seleccionado { get; set; }
        public Style nseleccionado { get; set; }
        public override Style SelectStyle(object item, System.Windows.DependencyObject container)
        {
            var entidad = item as HCAdquisicion;
            if (entidad.isSeleccionado)
                return this.seleccionado;
            else
                return this.nseleccionado;
        }
    }
    class SeleccionCarta: StyleSelector
    {
        public Style seleccionado { get; set; }
        public Style nseleccionado { get; set; }
        public override Style SelectStyle(object item, System.Windows.DependencyObject container)
        {
            var entidad = item as HCCarta;
            if (entidad.isSeleccionado)
                return this.seleccionado;
            else
                return this.nseleccionado;
        }
    }
    class SeleccionListProforma : StyleSelector
    {
        public Style seleccionado { get; set; }
        public Style nseleccionado { get; set; }
        public override Style SelectStyle(object item, System.Windows.DependencyObject container)
        {
            var entidad = item as List_Proforma;
            if (entidad.sedacion)
                return this.seleccionado;
            else
                return this.nseleccionado;
        }
    }
    class SeleccionListPreFactura : StyleSelector
    {
        public Style seleccionado { get; set; }
        public Style nseleccionado { get; set; }
        public override Style SelectStyle(object item, System.Windows.DependencyObject container)
        {
            var entidad = item as PreResumenFacGlobal;
            if (entidad.isSelected)
                return this.seleccionado;
            else
                return this.nseleccionado;
        }
    }
    
}
