import * as fs from 'fs';
import * as path from 'path';
import { spawn } from 'child_process';

const protosPath = path.join(__dirname, 'protos')
const protoDirEntries = fs.readdirSync(protosPath)
const destDir = "./src/generated/";
if (fs.existsSync(destDir)) {
    fs.rmSync(destDir, { force: true, recursive: true })
}
fs.mkdirSync(destDir)
protoDirEntries.forEach(entry => {
    console.log(entry);
    const args = `-I ./protos ./protos/${entry} --js_out=import_style=commonjs:${destDir} --grpc-web_out=import_style=typescript,mode=grpcwebtext:./src/generated/`
    spawn('protoc', args.split(' '))
});
