using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Globalization;
namespace DL_CustomCtrl
{
    class ShapeButtonConverter : ExpandableObjectConverter
    {
        //コンバータがオブジェクトを指定した型に変換できるか
        //（変換できる時はTrueを返す）
        //ここでは、CustomClass型のオブジェクトには変換可能とする
        public override bool CanConvertTo(
            ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(ShapeButton))
                return true;
            return base.CanConvertTo(context, destinationType);
        }
        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object instance, Attribute[] filters)
        {
            PropertyDescriptorCollection collection = new PropertyDescriptorCollection(null);

            PropertyDescriptorCollection properies = TypeDescriptor.GetProperties(instance, filters, true);
            foreach (PropertyDescriptor desc in properies)
            {
                collection.Add(new PropertyDisplayPropertyDescriptor(desc));
            }

            return collection;
        }

    }
}
