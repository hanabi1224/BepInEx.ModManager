import { app, BrowserWindow, Menu } from 'electron';
import logger from 'electron-log';
import { autoUpdater } from "electron-updater";
import path from 'path';
import { spawn } from "child_process";

const serverPort = 40003;
const cwd = process.cwd();
logger.info(cwd);
logger.info(__dirname);
process.chdir(path.join(cwd, 'out', 'server'))
spawn("BepInEx.ModManager.Server", `--server true --port ${serverPort}`.split(' '))
process.chdir(cwd)

autoUpdater.logger = logger;
autoUpdater.logger.transports.file.level = 'info';
logger.info('App starting...');

const template = []
let win: BrowserWindow

function sendStatusToWindow(text) {
   logger.info(text);
   win.webContents.send('message', text);
}

function createWindow() {
   win = new BrowserWindow({
      autoHideMenuBar: true,
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

app.on('ready', function () {
   // const menu = Menu.buildFromTemplate(template);
   // Menu.setApplicationMenu(menu);
   createWindow();
});

autoUpdater.on('checking-for-update', () => {
   sendStatusToWindow('Checking for update...');
})
autoUpdater.on('update-available', (info) => {
   sendStatusToWindow('Update available.');
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
});

app.on('ready', function () {
   autoUpdater.checkForUpdatesAndNotify();
});
