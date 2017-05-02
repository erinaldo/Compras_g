using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data;

namespace Datos.Clases
{
    public static class ControlsForms
    {
        //[System.ComponentModel.Bindable(true)]
        public static void setDataSource(ComboBox _control, DataTable _source, string _displayMember, string _valueMember, string _header)
        {
            if (!string.IsNullOrEmpty(_header))
            {
                DataRow _newRow = _source.NewRow();
                _newRow[_valueMember] = "0";
                _newRow[_displayMember] = _header;

                _source.Rows.InsertAt(_newRow, 0);
            }
            _control.DataSource = _source;
            _control.DisplayMember = _displayMember;
            _control.ValueMember = _valueMember;

        }

        public static void setDataSource(ListBox _control, DataTable _source, string _displayMember, string _valueMember, string _header)
        {


            if (!string.IsNullOrEmpty(_header))
            {
                DataRow _newRow = _source.NewRow();
                _newRow[_valueMember] = "0";
                _newRow[_displayMember] = _header;

                _source.Rows.InsertAt(_newRow, 0);
            }
            _control.DataSource = _source;
            _control.DisplayMember = _displayMember;
            _control.ValueMember = _valueMember;

        }

        public static void Autocomplete(TextBox txt, DataTable _source, string _name)
        {
            var source = new AutoCompleteStringCollection();
            source.AddRange((from item in _source.AsEnumerable()
                             select item.Field<string>(_name)).ToArray());

            txt.AutoCompleteCustomSource = source;
            txt.AutoCompleteMode = AutoCompleteMode.Suggest;
            txt.AutoCompleteSource = AutoCompleteSource.CustomSource;

        }


    }
}
