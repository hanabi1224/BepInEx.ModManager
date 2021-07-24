import { app, BrowserWindow, Menu } from 'electron';
import logger from 'electron-log';
import { autoUpdater } from "electron-updater";
import path from 'path';
import { spawn } from "child_process";

app.setAppUserModelId("BepInEx.ModManager");
app.setAsDefaultProtocolClient('biemm');

const serverPort = 40003;
const cwd = process.cwd();
logger.info(cwd);
logger.info(__dirname);
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

app.on('ready', async () => {
   const result = await autoUpdater.checkForUpdatesAndNotify();
   if (result) {
      logger.info(JSON.stringify(result));
   }
});
