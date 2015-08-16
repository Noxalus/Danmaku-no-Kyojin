using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Danmaku_no_Kyojin.Utils
{
    class ImprovementElement
    {
        private string _name;
        private List<KeyValuePair<object, int>> _content;
        private int _index;

        public ImprovementElement(string name, List<KeyValuePair<object, int>> content, int index)
        {
            _name = name;
            _content = content;
            _index = index;
        }

        public string Name
        {
            get { return _name; }
        }

        public object GetValue()
        {
            return _content[_index].Key;
        }

        public int GetPrice()
        {
            return _content[_index].Value;
        }

        public override string ToString()
        {
            return _name;
        }

    }
}
