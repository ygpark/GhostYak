// 
// Options.cs
// 
// Authors:
//  Young-Gi Park <ghostyak@gmail.com>
// 
// Copyright ©  2021 YOUNGGI PARK <ghostyak@gmail.com> ALL RIGHTS RESERVED.
// 
// 
// 
// 예제: 
//    int help = 0;
//    int list = 0;
//    int verbosity = 0;
//    int skip = 0;
//    string fileName = string.Empty;
//    string expression = string.Empty;
// 
//    OptionSet o = new OptionSet();
//    o.Add("h|help", "도움말", v => help++)
//        .Add("l|list", "모든 물리 디스크 목록 출력", v => list++)
//        .Add("v|verbose", "상세 정보 출력 모드", v => verbosity++)
//        .Add("i|ifile", "입력 파일 이름 또는 물리 디스크 이름", v => fileName = v.Trim())
//        .Add("e|regex", "정규표현식. -e\"regular expression\"", v => expression = v)
//        .Add("s|skip", "건너띄기 할 byte 수", v => skip = int.Parse(v));
//    
//    o.Parse(args);
//
//
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;



/// <summary>
/// 
/// </summary>
namespace GhostYak.IO.CommandLine.Options
{
    
    internal class OptionItem
    {
        private string[] _keys;
        private string _description = string.Empty;
        private Action<string> _action;
        private int _actionCount;
        private bool _isValueType = false;
        private string _prototype;



        public string[] Keys { get { return _keys; } set { _keys = value; } }
        
        public string Description { get { return _description; } set { _description = value; } }

        public string Prototype { get { return _prototype; } }

        public Action<string> Action 
        {
            get {
                _actionCount++;  
                return _action; 
            }
            set { _action = value; } 
        }

        /// <summary>
        /// Action 호출 횟수를 리턴합니다.
        /// </summary>
        public int ActionCount { get { return _actionCount; } }
        
        public bool IsValueType { get { return _isValueType; } }



        public void Add(string prototype, string description, Action<string> action)
        {
            if (action == null)
            {
                throw new Exception("Action<string> is null.");
            }
            _prototype = prototype;
            this.Keys = ParsePrototype(prototype);
            this.Description = description;
            this.Action = action;
        }

        private string[] ParsePrototype(string prototype)
        {
            string[] items = prototype.Split('|');
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i].Contains("="))
                {
                    _isValueType = true;
                    items[i] = items[i].Replace("=", "").Trim();
                } else {
                    items[i] = items[i].Trim();
                }

                items[i] = items[i].Replace("=", "").Trim();
            }
            return items;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("OptionSet { Keys: { ");
            for (int i = 0; i < _keys.Length - 1; i++)
            {
                sb.Append(_keys[i]);
                sb.Append(", ");
            }
            sb.Append(_keys[_keys.Length - 1]);
            sb.Append(" } ");
            sb.Append("Description: ");
            sb.Append(_description);
            sb.Append(" } ");

            return sb.ToString();
        }
    }




    public class OptionSet
    {
        private List<OptionItem> OptionItems;

        public OptionSet()
        {
            OptionItems = new List<OptionItem>();
        }

        public OptionSet Add(string prototype, string description, Action<string> action)
        {
            OptionItem set = new OptionItem();
            set.Add(prototype, description, action);
            OptionItems.Add(set);
            return this;
        }


        public void Parse(string[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                string arg = args[i];
                
                if (IsOptionFormat(arg))
                {
                    string key = GetKey(arg);
                    string value = GetValue(arg);

                    OptionItem item = FindOptionItemByKey(key);
                    if(item != null && item.ActionCount == 0)// 사용자 실수로 명령줄 인수가 -h -help 이런식으로 중복해서 생긴 경우 한번만 실행
                    {
                        item.Action(value);
                    }
                }
            }
        }



        private OptionItem FindOptionItemByKey(string key)
        {
            foreach (var item in this.OptionItems)
            {
                foreach (var registeredKey in item.Keys)
                {
                    if (key == registeredKey)
                        return item;
                }
            }

            return null;
        }


        /// <summary>
        /// 옵션에서 key를 찾아 리턴합니다.
        /// </summary>
        /// <param name="arg"> -KEY=VALUE 또는 --KEY=VALUE 또는 /KEY=VALUE 또는 -KEY 또는 --KEY 또는 /KEY </param>
        /// <returns>Key</returns>
        private string GetKey(string arg)
        {
            Match match = Regex.Match(arg, "^(-|--|/)([a-zA-Z]+)(=(.*)){0,1}$");
            return match.Groups[2].Value;
        }


        /// <summary>
        /// 옵션에서 value를 찾아 리턴합니다.
        /// </summary>
        /// <param name="arg"> -KEY=VALUE 또는 --KEY=VALUE 또는 /KEY=VALUE 또는 -KEY 또는 --KEY 또는 /KEY </param>
        /// <returns>VALUE</returns>
        private string GetValue(string arg)
        {
            Match match = Regex.Match(arg, "^(-|--|/)([a-zA-Z]+)(=(.*)){0,1}$");
            return match.Groups[4].Value;
        }


        /// <summary>
        /// 옵션 형식이 맞으면 true를 리턴합니다.
        /// </summary>
        /// <param name="arg"> -KEY=VALUE 또는 --KEY=VALUE 또는 /KEY=VALUE 또는 -KEY 또는 --KEY 또는 /KEY </param>
        /// <returns></returns>
        public bool IsOptionFormat(string arg)
        {
            return Regex.IsMatch(arg, "^(-|--|/)([a-zA-Z]+)(=(.*)){0,1}$");
        }



        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Option { ");
            for (int i = 0; i < OptionItems.Count - 1; i++)
            {
                sb.Append(OptionItems[i]);
                sb.Append(", ");
            }
            sb.Append(OptionItems[OptionItems.Count - 1]);
            sb.Append(" }");
            return sb.ToString();
        }



        public string GetOptionDescriptions()
        {
            // -h, --help     설명
            // -v, -verbose   설명

            StringBuilder sb = new StringBuilder();

            // 아래쪽 for문을 미리 돌려보면서 옵션부분의 문자열 최대 길이를 찾는다.
            // 아래쪽 for문이 변경되면 여기도 바꿔야 한다.
            int max = 0;
            for (int i = 0; i < OptionItems.Count; i++)
            {
                string head = "";
                for (int j = 0; j < OptionItems[i].Keys.Length; j++)
                {
                    string key = OptionItems[i].Keys[j];
                    string pre = key.Length == 1 ? "-" : "--";
                    head += pre;
                    head += key;
                    if (OptionItems[i].IsValueType)
                        head += "=VALUE";
                    if (j < OptionItems[i].Keys.Length - 1)
                        head += ", ";
                }
                if (max < head.Length)
                    max = head.Length;

            }

            for (int i = 0; i < OptionItems.Count; i++)
            {
                sb.Append("\t");
                int head = 0;
                for (int j = 0; j < OptionItems[i].Keys.Length; j++)
                {
                    string key = OptionItems[i].Keys[j];
                    string pre = key.Length == 1 ? "-" : "--";
                    sb.Append(pre); 
                    sb.Append(key);
                    head += pre.Length + key.Length;
                    if (OptionItems[i].IsValueType)
                    {
                        sb.Append("=VALUE");
                        head += "=VALUE".Length;

                    }
                    if (j < OptionItems[i].Keys.Length -1)
                    {
                        sb.Append(", ");
                        head += ", ".Length;
                    }
                }

                for (int k = 0; k < max - head; k++)
                {
                    sb.Append(" ");
                }

                sb.Append("\t");

                sb.Append(OptionItems[i].Description);

                if(i < OptionItems.Count -1)
                    sb.Append(System.Environment.NewLine);
            }

            return sb.ToString();
        }

    }
}
