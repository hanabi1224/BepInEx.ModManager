import * as jspb from 'google-protobuf'

import * as Game_pb from './Game_pb';


export class CheckPluginUpdatesRequest extends jspb.Message {
  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): CheckPluginUpdatesRequest.AsObject;
  static toObject(includeInstance: boolean, msg: CheckPluginUpdatesRequest): CheckPluginUpdatesRequest.AsObject;
  static serializeBinaryToWriter(message: CheckPluginUpdatesRequest, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): CheckPluginUpdatesRequest;
  static deserializeBinaryFromReader(message: CheckPluginUpdatesRequest, reader: jspb.BinaryReader): CheckPluginUpdatesRequest;
}

export namespace CheckPluginUpdatesRequest {
  export type AsObject = {
  }
}

export class InstallPluginRequest extends jspb.Message {
  getPluginpath(): string;
  setPluginpath(value: string): InstallPluginRequest;

  getGamepath(): string;
  setGamepath(value: string): InstallPluginRequest;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): InstallPluginRequest.AsObject;
  static toObject(includeInstance: boolean, msg: InstallPluginRequest): InstallPluginRequest.AsObject;
  static serializeBinaryToWriter(message: InstallPluginRequest, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): InstallPluginRequest;
  static deserializeBinaryFromReader(message: InstallPluginRequest, reader: jspb.BinaryReader): InstallPluginRequest;
}

export namespace InstallPluginRequest {
  export type AsObject = {
    pluginpath: string,
    gamepath: string,
  }
}

export class UninstallPluginRequest extends jspb.Message {
  getPluginpath(): string;
  setPluginpath(value: string): UninstallPluginRequest;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): UninstallPluginRequest.AsObject;
  static toObject(includeInstance: boolean, msg: UninstallPluginRequest): UninstallPluginRequest.AsObject;
  static serializeBinaryToWriter(message: UninstallPluginRequest, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): UninstallPluginRequest;
  static deserializeBinaryFromReader(message: UninstallPluginRequest, reader: jspb.BinaryReader): UninstallPluginRequest;
}

export namespace UninstallPluginRequest {
  export type AsObject = {
    pluginpath: string,
  }
}

export class AddPluginToRepoRequest extends jspb.Message {
  getData(): Uint8Array | string;
  getData_asU8(): Uint8Array;
  getData_asB64(): string;
  setData(value: Uint8Array | string): AddPluginToRepoRequest;

  getFilename(): string;
  setFilename(value: string): AddPluginToRepoRequest;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): AddPluginToRepoRequest.AsObject;
  static toObject(includeInstance: boolean, msg: AddPluginToRepoRequest): AddPluginToRepoRequest.AsObject;
  static serializeBinaryToWriter(message: AddPluginToRepoRequest, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): AddPluginToRepoRequest;
  static deserializeBinaryFromReader(message: AddPluginToRepoRequest, reader: jspb.BinaryReader): AddPluginToRepoRequest;
}

export namespace AddPluginToRepoRequest {
  export type AsObject = {
    data: Uint8Array | string,
    filename: string,
  }
}

export class ListPluginsRequest extends jspb.Message {
  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): ListPluginsRequest.AsObject;
  static toObject(includeInstance: boolean, msg: ListPluginsRequest): ListPluginsRequest.AsObject;
  static serializeBinaryToWriter(message: ListPluginsRequest, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): ListPluginsRequest;
  static deserializeBinaryFromReader(message: ListPluginsRequest, reader: jspb.BinaryReader): ListPluginsRequest;
}

export namespace ListPluginsRequest {
  export type AsObject = {
  }
}

export class ListPluginsResponse extends jspb.Message {
  getPluginsList(): Array<Game_pb.PluginInfo>;
  setPluginsList(value: Array<Game_pb.PluginInfo>): ListPluginsResponse;
  clearPluginsList(): ListPluginsResponse;
  addPlugins(value?: Game_pb.PluginInfo, index?: number): Game_pb.PluginInfo;

  getPatchersList(): Array<Game_pb.PatcherInfo>;
  setPatchersList(value: Array<Game_pb.PatcherInfo>): ListPluginsResponse;
  clearPatchersList(): ListPluginsResponse;
  addPatchers(value?: Game_pb.PatcherInfo, index?: number): Game_pb.PatcherInfo;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): ListPluginsResponse.AsObject;
  static toObject(includeInstance: boolean, msg: ListPluginsResponse): ListPluginsResponse.AsObject;
  static serializeBinaryToWriter(message: ListPluginsResponse, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): ListPluginsResponse;
  static deserializeBinaryFromReader(message: ListPluginsResponse, reader: jspb.BinaryReader): ListPluginsResponse;
}

export namespace ListPluginsResponse {
  export type AsObject = {
    pluginsList: Array<Game_pb.PluginInfo.AsObject>,
    patchersList: Array<Game_pb.PatcherInfo.AsObject>,
  }
}

export class ReadConfigRequest extends jspb.Message {
  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): ReadConfigRequest.AsObject;
  static toObject(includeInstance: boolean, msg: ReadConfigRequest): ReadConfigRequest.AsObject;
  static serializeBinaryToWriter(message: ReadConfigRequest, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): ReadConfigRequest;
  static deserializeBinaryFromReader(message: ReadConfigRequest, reader: jspb.BinaryReader): ReadConfigRequest;
}

export namespace ReadConfigRequest {
  export type AsObject = {
  }
}

export class ReadConfigResponse extends jspb.Message {
  getContent(): string;
  setContent(value: string): ReadConfigResponse;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): ReadConfigResponse.AsObject;
  static toObject(includeInstance: boolean, msg: ReadConfigResponse): ReadConfigResponse.AsObject;
  static serializeBinaryToWriter(message: ReadConfigResponse, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): ReadConfigResponse;
  static deserializeBinaryFromReader(message: ReadConfigResponse, reader: jspb.BinaryReader): ReadConfigResponse;
}

export namespace ReadConfigResponse {
  export type AsObject = {
    content: string,
  }
}

export class WriteConfigRequest extends jspb.Message {
  getContent(): string;
  setContent(value: string): WriteConfigRequest;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): WriteConfigRequest.AsObject;
  static toObject(includeInstance: boolean, msg: WriteConfigRequest): WriteConfigRequest.AsObject;
  static serializeBinaryToWriter(message: WriteConfigRequest, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): WriteConfigRequest;
  static deserializeBinaryFromReader(message: WriteConfigRequest, reader: jspb.BinaryReader): WriteConfigRequest;
}

export namespace WriteConfigRequest {
  export type AsObject = {
    content: string,
  }
}

export class AddGameRequest extends jspb.Message {
  getPath(): string;
  setPath(value: string): AddGameRequest;

  getName(): string;
  setName(value: string): AddGameRequest;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): AddGameRequest.AsObject;
  static toObject(includeInstance: boolean, msg: AddGameRequest): AddGameRequest.AsObject;
  static serializeBinaryToWriter(message: AddGameRequest, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): AddGameRequest;
  static deserializeBinaryFromReader(message: AddGameRequest, reader: jspb.BinaryReader): AddGameRequest;
}

export namespace AddGameRequest {
  export type AsObject = {
    path: string,
    name: string,
  }
}

