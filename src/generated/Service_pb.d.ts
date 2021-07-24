import * as jspb from 'google-protobuf'

import * as Common_pb from './Common_pb';
import * as Game_pb from './Game_pb';
import * as Repo_pb from './Repo_pb';


export class LongConnectRequest extends jspb.Message {
  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): LongConnectRequest.AsObject;
  static toObject(includeInstance: boolean, msg: LongConnectRequest): LongConnectRequest.AsObject;
  static serializeBinaryToWriter(message: LongConnectRequest, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): LongConnectRequest;
  static deserializeBinaryFromReader(message: LongConnectRequest, reader: jspb.BinaryReader): LongConnectRequest;
}

export namespace LongConnectRequest {
  export type AsObject = {
  }
}

export class LongConnectResponse extends jspb.Message {
  getMessage(): string;
  setMessage(value: string): LongConnectResponse;

  getNotification(): ServerSideNotification;
  setNotification(value: ServerSideNotification): LongConnectResponse;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): LongConnectResponse.AsObject;
  static toObject(includeInstance: boolean, msg: LongConnectResponse): LongConnectResponse.AsObject;
  static serializeBinaryToWriter(message: LongConnectResponse, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): LongConnectResponse;
  static deserializeBinaryFromReader(message: LongConnectResponse, reader: jspb.BinaryReader): LongConnectResponse;
}

export namespace LongConnectResponse {
  export type AsObject = {
    message: string,
    notification: ServerSideNotification,
  }
}

export enum ServerSideNotification { 
  UNKNOWN = 0,
  REFRESHGAMEINFO = 1,
  REFRESHREPOINFO = 2,
}
