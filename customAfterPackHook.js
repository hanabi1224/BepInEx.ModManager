const { spawn } = require('child_process');
const fs = require('fs');
const path = require('path')

exports.default = async function (context) {
    const serverOutDir = path.join(context.appOutDir, 'out')
    fs.mkdirSync(serverOutDir)
    spawn('cp', `-r out/server ${serverOutDir}`.split(' '))
    // console.log(JSON.stringify(context));
}
