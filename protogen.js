"use strict";
exports.__esModule = true;
var fs = require("fs");
var path = require("path");
var child_process_1 = require("child_process");
var protosPath = path.join(__dirname, 'protos');
var protoDirEntries = fs.readdirSync(protosPath);
var destDir = "./src/generated/";
if (fs.existsSync(destDir)) {
    fs.rmSync(destDir, { force: true, recursive: true });
}
fs.mkdirSync(destDir);
protoDirEntries.forEach(function (entry) {
    console.log(entry);
    var args = "-I ./protos ./protos/" + entry + " --js_out=import_style=commonjs:" + destDir + " --grpc-web_out=import_style=typescript,mode=grpcwebtext:./src/generated/";
    child_process_1.spawn('protoc', args.split(' '));
});
