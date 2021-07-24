import { ClientReadableStream } from "grpc-web";
import { ModManagerServiceClient } from "./generated/ServiceServiceClientPb";
import { LongConnectRequest, LongConnectResponse } from "./generated/Service_pb";

// TODO: parse server port via url.
const serverPort = 40003;
export const grpcClient = new ModManagerServiceClient(`http://localhost:${serverPort}`, null, null);

export const longConnectStream = grpcClient.longConnect(new LongConnectRequest(), {}) as ClientReadableStream<LongConnectResponse>;
// longConnectStream
//     .on('status', (status)=>{
//         console.log(status.code)
//     })
//     .on('data', (response)=>{
//     // console.log(response.getMessage())
//     })
//     .on('error', (err)=>{
//         console.log(err)
//     });
