import { app, BrowserWindow, Menu, MenuItem, MenuItemConstructorOptions, shell } from 'electron';
import logger from 'electron-log';
import { autoUpdater } from "electron-updater";
import path from 'path';
import { spawn } from "child_process";
import osLocale from 'os-locale';
import i18next from 'i18next';
import i18nConfig from './i18n/config';

i18next.init(i18nConfig)

app.setAppUserModelId("BepInEx.ModManager");
app.setAsDefaultProtocolClient('biemm');

const serverPort = 40003;
const cwd = process.cwd();
logger.info(cwd);
const serverExe = path.join(cwd, 'out', 'server', 'BepInEx.ModManager.Server.exe')
const server = spawn(serverExe, `--server true --port ${serverPort}`.split(' '))
server.stdout.on("data", function (data) {
   console.log(data.toString());
});
server.stdout.on("error", function (err) {
   console.error(err.toString());
});

autoUpdater.logger = logger;
autoUpdater.logger.transports.file.level = 'info';
logger.info('App starting...');

let win: BrowserWindow

function changeLang(lang: 'en' | 'zh') {
   if (i18next.language != lang) {
      i18next.changeLanguage(lang)
      win?.webContents.send('set-lang', lang)
      drawMenuBar()
   }
}

function drawMenuBar() {
   const lang = i18next.language
   const menu = Menu.buildFromTemplate([
      {
         label: i18next.t("Operations"),
         submenu: [
            { role: 'toggleDevTools', label: i18next.t('Developer Tool') },
            { role: 'quit', label: i18next.t('Exit') },
         ],
      },
      {
         label: i18next.t('Language Setting'),
         submenu: [
            { label: 'English', type: "checkbox", checked: lang == 'en', click: () => changeLang('en') },
            { type: 'separator' },
            { label: '简体中文', type: "checkbox", checked: lang == 'zh', click: () => changeLang('zh') },
         ]
      }, {
         label: i18next.t('About'),
         submenu: [
            { label: i18next.t('Check For Update'), click: () => shell.openPath('https://github.com/hanabi1224/BepInEx.ModManager/releases/latest') },
            { label: 'Github', click: () => shell.openPath('https://github.com/hanabi1224/BepInEx.ModManager'), }
         ]
      }
   ]);
   Menu.setApplicationMenu(menu);
}

function sendStatusToWindow(text) {
   logger.info(text);
}

function createWindow() {
   win = new BrowserWindow({
      // autoHideMenuBar: true,
      fullscreenable: false,
      frame: true,
      webPreferences: {
         nodeIntegration: true,
         contextIsolation: false,
      }
   })
   win.loadURL(`file://${__dirname}/index.html#v${app.getVersion()}`)
   win.maximize()
}

app.on('window-all-closed', () => {
   app.quit();
});

app.on('ready', async function () {
   const locale = await osLocale();
   // const locale = app.getLocale();
   console.log(`locale: ${locale}`);
   changeLang(locale.split('-')[0]);
   createWindow();
});

autoUpdater.on('checking-for-update', () => {
   sendStatusToWindow('Checking for update...');
})
autoUpdater.on('update-available', (info) => {
   sendStatusToWindow('Update available.');
   win.webContents.send('update-available', info);
})
autoUpdater.on('update-not-available', (info) => {
   sendStatusToWindow('Update not available.');
})
autoUpdater.on('error', (err) => {
   sendStatusToWindow('Error in auto-updater. ' + err);
})
autoUpdater.on('download-progress', (progressObj) => {
   let log_message = "Download speed: " + progressObj.bytesPerSecond;
   log_message = log_message + ' - Downloaded ' + progressObj.percent + '%';
   log_message = log_message + ' (' + progressObj.transferred + "/" + progressObj.total + ')';
   sendStatusToWindow(log_message);
})
autoUpdater.on('update-downloaded', (info) => {
   sendStatusToWindow('Update downloaded');
   win.webContents.send('update-downloaded', info);
});

app.on('ready', async () => {
   const result = await autoUpdater.checkForUpdatesAndNotify();
   if (result) {
      logger.info(JSON.stringify(result));
   }
});
