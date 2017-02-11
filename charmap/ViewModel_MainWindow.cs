using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using System.Linq;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using charmap.Input;
using System.Windows;

namespace charmap.ViewModel
{
    public class ViewModel_MainWindow : INotifyPropertyChanged
    {
        #region properties
        private ObservableCollection<string> _Characters = new ObservableCollection<string>();
        public ObservableCollection<string> Characters
        {
            get { return _Characters; }
            set
            {
                if (_Characters != value)
                {
                    _Characters = value;
                    RaisePropertyChanged("Characters");
                }
            }
        }

        private ObservableCollection<FontFamily> _FontFamilyNames = new ObservableCollection<FontFamily>();
        public ObservableCollection<FontFamily> FontFamilyNames
        {
            get { return _FontFamilyNames; }
            set
            {
                if (_FontFamilyNames != value)
                {
                    _FontFamilyNames = value;
                    RaisePropertyChanged("FontFamilyNames");
                }
            }
        }

        private FontFamily _SelectedFontFamily = new FontFamily();
        public FontFamily SelectedFontFamily
        {
            get { return _SelectedFontFamily; }
            set
            {
                if (_SelectedFontFamily != value)
                {
                    _SelectedFontFamily = value;
                    RaisePropertyChanged("SelectedFontFamily");

                    Refresh();
                }
            }
        }

        private string _SelectedCharacter = string.Empty;
        public string SelectedCharacter
        {
            get { return _SelectedCharacter; }
            set
            {
                if (_SelectedCharacter != value)
                {
                    _SelectedCharacter = value;
                    RaisePropertyChanged("SelectedCharacter");
                }
            }
        }

        private bool _ShowProgressBar = false;
        public bool ShowProgressBar
        {
            get { return _ShowProgressBar; }
            set
            {
                if (_ShowProgressBar != value)
                {
                    _ShowProgressBar = value;
                    RaisePropertyChanged("ShowProgressBar");
                }
            }
        }

        private bool _ShowSelectedCharacter = false;
        public bool ShowSelectedCharacter
        {
            get { return _ShowSelectedCharacter; }
            set
            {
                if (_ShowSelectedCharacter != value)
                {
                    _ShowSelectedCharacter = value;
                    RaisePropertyChanged("ShowSelectedCharacter");
                }
            }
        }

        private string _SelectedCharacters = string.Empty;
        public string SelectedCharacters
        {
            get { return _SelectedCharacters; }
            set
            {
                if (_SelectedCharacters != value)
                {
                    _SelectedCharacters = value;
                    RaisePropertyChanged("SelectedCharacters");
                }
            }
        }
        #endregion

        #region commands
        public ICommand Command_SelectCharacter { get; set; }
        public ICommand Command_ShowSelectedCharacter { get; set; }
        public ICommand Command_Copy { get; set; }
        public ICommand Command_AddToCharactersToCopy { get; set; }
        public ICommand Command_CopyAll { get; set; }
        #endregion

        #region ctor
        public ViewModel_MainWindow()
        {
            if (Command_SelectCharacter == null) Command_SelectCharacter = new RelayCommand<string>(Command_SelectCharacter_Click);
            if (Command_ShowSelectedCharacter == null) Command_ShowSelectedCharacter = new RelayCommand<string>(Command_SelectCharacter_Click);
            if (Command_Copy == null) Command_Copy = new RelayCommand(Command_Copy_Click);
            if (Command_AddToCharactersToCopy == null) Command_AddToCharactersToCopy = new RelayCommand(Command_AddToCharactersToCopy_Click);
            if (Command_CopyAll == null) Command_CopyAll = new RelayCommand(Command_CopyAll_Click);

            ICollection<FontFamily> ICollectionFonts = Fonts.SystemFontFamilies;
            List<FontFamily> fonts = ICollectionFonts.ToList();
            fonts.ForEach(x =>
            {
                this.FontFamilyNames.Add(x);
            });
            this.SelectedFontFamily = this.FontFamilyNames[0];
            //List<string> fontNames = new List<string>();
            //fonts.ForEach(x =>
            //{
            //    fontNames.Add(x.ToString());
            //});
            //fontNames.Sort();
            //fontNames.ForEach(x =>
            //{
            //    this.FontFamilyNames.Add(x);
            //});
        }
        #endregion

        #region command methods
        void Command_SelectCharacter_Click(string selectedChar)
        {
            this.SelectedCharacter = selectedChar;
            Command_ShowSelectedCharacter_Click();
        }

        void Command_ShowSelectedCharacter_Click()
        {
            this.ShowSelectedCharacter = !this.ShowSelectedCharacter;
        }

        void Command_Copy_Click()
        {
            Clipboard.SetText(this.SelectedCharacter);
        }

        void Command_AddToCharactersToCopy_Click()
        {
            this.SelectedCharacters += this.SelectedCharacter;
        }

        void Command_CopyAll_Click()
        {
            Clipboard.SetText(this.SelectedCharacters);
        }
        #endregion

        #region methods
        public async void Refresh()
        {
            this.ShowProgressBar = true;

            var t = await Task.Run<List<string>>(() =>
            {
                List<string> chars = new List<string>();
                GlyphTypeface glyph;
                IDictionary<int, ushort> characterMap;

                var typefaces = this.SelectedFontFamily.GetTypefaces();
                foreach (Typeface typeface in typefaces)
                {
                    typeface.TryGetGlyphTypeface(out glyph);
                    if (glyph != null)
                    {
                        characterMap = glyph.CharacterToGlyphMap;

                        chars.Clear();
                        for (int i = 0; i < characterMap.Count; i++)
                        {
                            long index = characterMap.Keys.ElementAt(i);

                            try
                            {
                                char c = Convert.ToChar(index);
                                chars.Add(c.ToString());
                            }
                            catch(Exception ex)
                            {
                                chars.Add(string.Empty);
                            }
                        }
                    }
                }

                return chars;
            });

            this.Characters.Clear();
            t.ForEach(x =>
            {
                this.Characters.Add(x);
            });
            //t.Result.ForEach(x =>
            //{
            //    this.Characters.Add(x);
            //});

            this.ShowProgressBar = false;
        }
        #endregion

        #region INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;
        void RaisePropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
