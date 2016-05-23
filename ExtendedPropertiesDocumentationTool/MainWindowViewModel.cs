using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

using System.Windows;
using DatabaseMetadata.DataAccess;
using DatabaseMetadata.Entities;
using System.Data.Common;

namespace ExtendedPropertiesDocumentationTool
{
    public class MainWindowViewModel : System.ComponentModel.INotifyPropertyChanged
    {
        DatabaseMetaData _dbMetaData;
        StoredProcedureMetaData _selectedSPMetadata;
        MetadataFacade _metaAccess;
        

        public DelegateCommand<TableMetadata> SaveTableCommand { get; private set; }
        public DelegateCommand<ViewMetadata> SaveViewCommand { get; private set; }
        public DelegateCommand LoadDatabaseCommand { get; private set; }
        public DelegateCommand CreateWikiMarkupForTablesCommand { get; private set; }
        public DelegateCommand CreateWikiMarkupFoViewsCommand { get; private set; }
        public DelegateCommand CreateWikiMarkupForSPsCommand { get; private set; }
        public DelegateCommand<TableMetadata> CreateWikiMarkupForSelectedTableCommand { get; private set; }
        public DelegateCommand<ViewMetadata> CreateWikiMarkupForSelectedViewCommand { get; private set; }
        public DelegateCommand OpenSqlConnectionBuilder { get; private set; }

        public DelegateCommand<TableMetadata> CreateSQLStatementsForSelectedTableCommand { get; private set; }

        public DelegateCommand<StoredProcedureMetaData> CreateWikiMarkupForSelectedStoredProcedureCommand { get; private set; }
        public DelegateCommand<StoredProcedureMetaData> SaveStoreProcedureCommand { get; private set; }

        public DelegateCommand<TableMetadata> CreateWikiMarkupForIndexOnSelectedTableCommand { get; private set; }
        public DelegateCommand CreateWikiMarkupForAllIndexesCommand { get; private set; }
        public DelegateCommand OpenOptionsCommand { get; private set; }

        public StoredProcedureMetaData SelectedStoredProcedure
          {
            get
            {
                return _selectedSPMetadata;
            }
            set
            {
                _selectedSPMetadata = value;
                if(_selectedSPMetadata != null)
                    _metaAccess.GetStoredProcedureMetaDataDetails(_selectedSPMetadata, ConnectionString);
                OnPropertyChanged("SelectedStoredProcedure");
            }
        }

        public DatabaseMetaData DatabaseMetaData
        {
            get
            {
                return _dbMetaData;
            }
            set
            {
                _dbMetaData = value;
                OnPropertyChanged("DatabaseMetaData");
            }
        }


        ViewMetadata _selectedView;
        public ViewMetadata SelectedView
        {
            get
            {
                return _selectedView;
            }
            set
            {
                if (_selectedView != value)
                {
                    _selectedView = value;
                    OnPropertyChanged("SelectedView");
                }
            }
        }

        TableMetadata _selectedTableMetaData;
        public TableMetadata SelectedTable { 
            get 
            { 
                return _selectedTableMetaData; 
            } 
            set 
            {
                // TODO: Warn about unsaved changes.
                _selectedTableMetaData = value;
                if (_selectedTableMetaData != null)
                    _metaAccess.GetTableMetaDetails(_selectedTableMetaData, ConnectionString);               
                OnPropertyChanged("SelectedTable");
            } 
        }


        public string ConnectionString
        { 
            get 
            {
                string connStr = DocumentationTool.Default.ConnectionString;
                if (string.IsNullOrEmpty(connStr))
                {
                    connStr = "Data Source=LOCALHOST;Initial Catalog=Master;Integrated Security=SSPI;";
                }
                return connStr;
            } 
            set 
            {
                DocumentationTool.Default.ConnectionString = value;
                DocumentationTool.Default.Save();
                OnPropertyChanged("ConnectionString");
            } 
        }

        private bool _isLoaded = false;
        public bool IsLoaded
        {
            get
            {
                return _isLoaded;
            }
            set
            {
                _isLoaded = value;
                OnPropertyChanged("IsLoaded");
            }
        }

        //private ApplicationSettings _appSettings;
        public ApplicationSettings AppSettings
        {
            get
            {
                return ApplicationSettings.Default;
            }
        }
        

        public void ReloadDBMetaData(string connStr)
        {
            DatabaseMetaData = _metaAccess.GetMetaDataForDatabase(connStr);
            SelectedTable = DatabaseMetaData.Tables.FirstOrDefault();
            SelectedStoredProcedure = DatabaseMetaData.StoredProcedures.FirstOrDefault();
            SelectedView = DatabaseMetaData.Views.FirstOrDefault();
            IsLoaded = true;
        }

        public void SaveChangesForTable(TableMetadata table, string connStr)
        {
            if(table != null)
                _metaAccess.SaveTableMetaData(table, connStr);
        }

        public MainWindowViewModel()
        {
            _metaAccess = new MetadataFacade();

            SaveTableCommand = new DelegateCommand<TableMetadata>(tmd =>
                {
                    SaveChangesForTable(tmd, ConnectionString);
                });
           
            LoadDatabaseCommand = new DelegateCommand( () =>
                {
                    try
                    {
                        ReloadDBMetaData(ConnectionString);
                    }
                    catch (DbException ex)
                    {
                        MessageBox.Show("Could not connect to the database, make sure you have entered the correct information: " + ex.ToString());
                    }
                    catch (Exception exx)
                    {
                        MessageBox.Show("Critical error somewhere: " + exx.ToString());
                    }
                });

            CreateWikiMarkupForTablesCommand = new DelegateCommand( () =>
                {
                    string result = _metaAccess.GenerateWikiMarkupForTablesAndViews(ConnectionString, null, Level1Types.Table);
                    ModalWindows.WikiMarkupDisplay disp = new ModalWindows.WikiMarkupDisplay();
                    disp.Show(result);
                });

            CreateWikiMarkupForAllIndexesCommand = new DelegateCommand(() =>
            {
                string result = _metaAccess.CreateWikiMarkupForIndexesOnAllTables(ConnectionString);
                ModalWindows.WikiMarkupDisplay disp = new ModalWindows.WikiMarkupDisplay();
                disp.Show(result);
            });
            

            CreateWikiMarkupForSPsCommand = new DelegateCommand( () =>
            {
                string result = _metaAccess.GenerateWikiMarkupForStoredProcedures(ConnectionString);
                ModalWindows.WikiMarkupDisplay disp = new ModalWindows.WikiMarkupDisplay();
                disp.Show(result);
            });

            CreateWikiMarkupForSelectedTableCommand = new DelegateCommand<TableMetadata>(tmd =>
                {
                    string result = _metaAccess.GenerateWikiMarkupForTablesAndViews(ConnectionString, tmd, Level1Types.Table);
                    ModalWindows.WikiMarkupDisplay disp = new ModalWindows.WikiMarkupDisplay();
                    disp.Show(result);
                });
            
            CreateWikiMarkupForIndexOnSelectedTableCommand = new DelegateCommand<TableMetadata>(tmd =>
                {
                    string result = _metaAccess.CreateWikiMarkupForIndexesOnTable(tmd, ConnectionString);
                    ModalWindows.WikiMarkupDisplay disp = new ModalWindows.WikiMarkupDisplay();
                    disp.Show(result);
                });


            CreateWikiMarkupForSelectedStoredProcedureCommand = new DelegateCommand<StoredProcedureMetaData>(spMetadata =>
                {
                    string result = _metaAccess.GenerateWikiMarkupForSelectedStoredProcedure(spMetadata, ConnectionString);
                    ModalWindows.WikiMarkupDisplay disp = new ModalWindows.WikiMarkupDisplay();
                    disp.Show(result);
                });

            SaveStoreProcedureCommand = new DelegateCommand<StoredProcedureMetaData>(spMetadata =>
                {
                    _metaAccess.SaveStoredProcedureMetadata(_selectedSPMetadata, ConnectionString);
                });


            OpenSqlConnectionBuilder = new DelegateCommand(() =>
                {
                    ConnectionStringCreatorGUI.SqlConnectionString initialConnStr;

                    try
                    {
                        initialConnStr = new ConnectionStringCreatorGUI.SqlConnectionString(ConnectionString);
                    }
                    catch (Exception)
                    {
                        initialConnStr = new ConnectionStringCreatorGUI.SqlConnectionString();
                    }
                 
                    Window win = new ConnectionStringCreatorGUI.ConnectionStringBuilderWindow(initialConnStr, returnConnBuilder =>
                    {
                        ConnectionString = returnConnBuilder.ToString();
                    });

                    win.Show();

                }  );

                OpenOptionsCommand = new DelegateCommand(() =>
                {
                    Window win = new ExtendedPropertiesDocumentationTool.ModalWindows.ConfigurationWindow(AppSettings);
                    win.Show();
                });
          

            SaveViewCommand = new DelegateCommand<ViewMetadata>( vw =>
                {
                    _metaAccess.SaveViewMetaData(vw, ConnectionString);
                });
           
            CreateWikiMarkupForSelectedViewCommand = new DelegateCommand<ViewMetadata>(vw =>
            {
                string result = _metaAccess.GenerateWikiMarkupForTablesAndViews(ConnectionString, vw, Level1Types.View);
                ModalWindows.WikiMarkupDisplay disp = new ModalWindows.WikiMarkupDisplay();
                disp.Show(result);
            });
            CreateWikiMarkupFoViewsCommand = new DelegateCommand(() =>
                {
                    string result = _metaAccess.GenerateWikiMarkupForTablesAndViews(ConnectionString, null, Level1Types.View);
                    ModalWindows.WikiMarkupDisplay disp = new ModalWindows.WikiMarkupDisplay();
                    disp.Show(result);
                });

            CreateSQLStatementsForSelectedTableCommand = new DelegateCommand<TableMetadata>(tmd =>
            {
                string result = _metaAccess.GenerateSQLStatementForTable(ConnectionString, tmd, Level1Types.Table);
                ModalWindows.WikiMarkupDisplay disp = new ModalWindows.WikiMarkupDisplay();
                disp.Show(result);
            });
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if(PropertyChanged != null)
                PropertyChanged.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));      
        }
    }
}
