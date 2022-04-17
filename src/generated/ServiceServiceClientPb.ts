/**
 * @fileoverview gRPC-Web generated client stub for BepInEx
 * @enhanceable
 * @public
 */

// GENERATED CODE -- DO NOT EDIT!


/* eslint-disable */
// @ts-nocheck


import * as grpcWeb from 'grpc-web';

import * as Repo_pb from './Repo_pb';
import * as Common_pb from './Common_pb';
import * as Game_pb from './Game_pb';
import * as Service_pb from './Service_pb';


export class ModManagerServiceClient {
  client_: grpcWeb.AbstractClientBase;
  hostname_: string;
  credentials_: null | { [index: string]: string; };
  options_: null | { [index: string]: any; };

  constructor (hostname: string,
               credentials?: null | { [index: string]: string; },
               options?: null | { [index: string]: any; }) {
    if (!options) options = {};
    if (!credentials) credentials = {};
    options['format'] = 'text';

    this.client_ = new grpcWeb.GrpcWebClientBase(options);
    this.hostname_ = hostname;
    this.credentials_ = credentials;
    this.options_ = options;
  }

  methodDescriptorListGames = new grpcWeb.MethodDescriptor(
    '/BepInEx.ModManagerService/ListGames',
    grpcWeb.MethodType.UNARY,
    Game_pb.ListGamesRequest,
    Game_pb.ListGamesResponse,
    (request: Game_pb.ListGamesRequest) => {
      return request.serializeBinary();
    },
    Game_pb.ListGamesResponse.deserializeBinary
  );

  listGames(
    request: Game_pb.ListGamesRequest,
    metadata: grpcWeb.Metadata | null): Promise<Game_pb.ListGamesResponse>;

  listGames(
    request: Game_pb.ListGamesRequest,
    metadata: grpcWeb.Metadata | null,
    callback: (err: grpcWeb.RpcError,
               response: Game_pb.ListGamesResponse) => void): grpcWeb.ClientReadableStream<Game_pb.ListGamesResponse>;

  listGames(
    request: Game_pb.ListGamesRequest,
    metadata: grpcWeb.Metadata | null,
    callback?: (err: grpcWeb.RpcError,
               response: Game_pb.ListGamesResponse) => void) {
    if (callback !== undefined) {
      return this.client_.rpcCall(
        this.hostname_ +
          '/BepInEx.ModManagerService/ListGames',
        request,
        metadata || {},
        this.methodDescriptorListGames,
        callback);
    }
    return this.client_.unaryCall(
    this.hostname_ +
      '/BepInEx.ModManagerService/ListGames',
    request,
    metadata || {},
    this.methodDescriptorListGames);
  }

  methodDescriptorInstallBIE = new grpcWeb.MethodDescriptor(
    '/BepInEx.ModManagerService/InstallBIE',
    grpcWeb.MethodType.UNARY,
    Game_pb.InstallBIERequest,
    Common_pb.CommonServiceResponse,
    (request: Game_pb.InstallBIERequest) => {
      return request.serializeBinary();
    },
    Common_pb.CommonServiceResponse.deserializeBinary
  );

  installBIE(
    request: Game_pb.InstallBIERequest,
    metadata: grpcWeb.Metadata | null): Promise<Common_pb.CommonServiceResponse>;

  installBIE(
    request: Game_pb.InstallBIERequest,
    metadata: grpcWeb.Metadata | null,
    callback: (err: grpcWeb.RpcError,
               response: Common_pb.CommonServiceResponse) => void): grpcWeb.ClientReadableStream<Common_pb.CommonServiceResponse>;

  installBIE(
    request: Game_pb.InstallBIERequest,
    metadata: grpcWeb.Metadata | null,
    callback?: (err: grpcWeb.RpcError,
               response: Common_pb.CommonServiceResponse) => void) {
    if (callback !== undefined) {
      return this.client_.rpcCall(
        this.hostname_ +
          '/BepInEx.ModManagerService/InstallBIE',
        request,
        metadata || {},
        this.methodDescriptorInstallBIE,
        callback);
    }
    return this.client_.unaryCall(
    this.hostname_ +
      '/BepInEx.ModManagerService/InstallBIE',
    request,
    metadata || {},
    this.methodDescriptorInstallBIE);
  }

  methodDescriptorUninstallBIE = new grpcWeb.MethodDescriptor(
    '/BepInEx.ModManagerService/UninstallBIE',
    grpcWeb.MethodType.UNARY,
    Game_pb.UninstallBIERequest,
    Common_pb.CommonServiceResponse,
    (request: Game_pb.UninstallBIERequest) => {
      return request.serializeBinary();
    },
    Common_pb.CommonServiceResponse.deserializeBinary
  );

  uninstallBIE(
    request: Game_pb.UninstallBIERequest,
    metadata: grpcWeb.Metadata | null): Promise<Common_pb.CommonServiceResponse>;

  uninstallBIE(
    request: Game_pb.UninstallBIERequest,
    metadata: grpcWeb.Metadata | null,
    callback: (err: grpcWeb.RpcError,
               response: Common_pb.CommonServiceResponse) => void): grpcWeb.ClientReadableStream<Common_pb.CommonServiceResponse>;

  uninstallBIE(
    request: Game_pb.UninstallBIERequest,
    metadata: grpcWeb.Metadata | null,
    callback?: (err: grpcWeb.RpcError,
               response: Common_pb.CommonServiceResponse) => void) {
    if (callback !== undefined) {
      return this.client_.rpcCall(
        this.hostname_ +
          '/BepInEx.ModManagerService/UninstallBIE',
        request,
        metadata || {},
        this.methodDescriptorUninstallBIE,
        callback);
    }
    return this.client_.unaryCall(
    this.hostname_ +
      '/BepInEx.ModManagerService/UninstallBIE',
    request,
    metadata || {},
    this.methodDescriptorUninstallBIE);
  }

  methodDescriptorCheckPluginUpdates = new grpcWeb.MethodDescriptor(
    '/BepInEx.ModManagerService/CheckPluginUpdates',
    grpcWeb.MethodType.UNARY,
    Repo_pb.CheckPluginUpdatesRequest,
    Common_pb.CommonServiceResponse,
    (request: Repo_pb.CheckPluginUpdatesRequest) => {
      return request.serializeBinary();
    },
    Common_pb.CommonServiceResponse.deserializeBinary
  );

  checkPluginUpdates(
    request: Repo_pb.CheckPluginUpdatesRequest,
    metadata: grpcWeb.Metadata | null): Promise<Common_pb.CommonServiceResponse>;

  checkPluginUpdates(
    request: Repo_pb.CheckPluginUpdatesRequest,
    metadata: grpcWeb.Metadata | null,
    callback: (err: grpcWeb.RpcError,
               response: Common_pb.CommonServiceResponse) => void): grpcWeb.ClientReadableStream<Common_pb.CommonServiceResponse>;

  checkPluginUpdates(
    request: Repo_pb.CheckPluginUpdatesRequest,
    metadata: grpcWeb.Metadata | null,
    callback?: (err: grpcWeb.RpcError,
               response: Common_pb.CommonServiceResponse) => void) {
    if (callback !== undefined) {
      return this.client_.rpcCall(
        this.hostname_ +
          '/BepInEx.ModManagerService/CheckPluginUpdates',
        request,
        metadata || {},
        this.methodDescriptorCheckPluginUpdates,
        callback);
    }
    return this.client_.unaryCall(
    this.hostname_ +
      '/BepInEx.ModManagerService/CheckPluginUpdates',
    request,
    metadata || {},
    this.methodDescriptorCheckPluginUpdates);
  }

  methodDescriptorListPlugins = new grpcWeb.MethodDescriptor(
    '/BepInEx.ModManagerService/ListPlugins',
    grpcWeb.MethodType.UNARY,
    Repo_pb.ListPluginsRequest,
    Repo_pb.ListPluginsResponse,
    (request: Repo_pb.ListPluginsRequest) => {
      return request.serializeBinary();
    },
    Repo_pb.ListPluginsResponse.deserializeBinary
  );

  listPlugins(
    request: Repo_pb.ListPluginsRequest,
    metadata: grpcWeb.Metadata | null): Promise<Repo_pb.ListPluginsResponse>;

  listPlugins(
    request: Repo_pb.ListPluginsRequest,
    metadata: grpcWeb.Metadata | null,
    callback: (err: grpcWeb.RpcError,
               response: Repo_pb.ListPluginsResponse) => void): grpcWeb.ClientReadableStream<Repo_pb.ListPluginsResponse>;

  listPlugins(
    request: Repo_pb.ListPluginsRequest,
    metadata: grpcWeb.Metadata | null,
    callback?: (err: grpcWeb.RpcError,
               response: Repo_pb.ListPluginsResponse) => void) {
    if (callback !== undefined) {
      return this.client_.rpcCall(
        this.hostname_ +
          '/BepInEx.ModManagerService/ListPlugins',
        request,
        metadata || {},
        this.methodDescriptorListPlugins,
        callback);
    }
    return this.client_.unaryCall(
    this.hostname_ +
      '/BepInEx.ModManagerService/ListPlugins',
    request,
    metadata || {},
    this.methodDescriptorListPlugins);
  }

  methodDescriptorAddPluginToRepo = new grpcWeb.MethodDescriptor(
    '/BepInEx.ModManagerService/AddPluginToRepo',
    grpcWeb.MethodType.UNARY,
    Repo_pb.AddPluginToRepoRequest,
    Common_pb.CommonServiceResponse,
    (request: Repo_pb.AddPluginToRepoRequest) => {
      return request.serializeBinary();
    },
    Common_pb.CommonServiceResponse.deserializeBinary
  );

  addPluginToRepo(
    request: Repo_pb.AddPluginToRepoRequest,
    metadata: grpcWeb.Metadata | null): Promise<Common_pb.CommonServiceResponse>;

  addPluginToRepo(
    request: Repo_pb.AddPluginToRepoRequest,
    metadata: grpcWeb.Metadata | null,
    callback: (err: grpcWeb.RpcError,
               response: Common_pb.CommonServiceResponse) => void): grpcWeb.ClientReadableStream<Common_pb.CommonServiceResponse>;

  addPluginToRepo(
    request: Repo_pb.AddPluginToRepoRequest,
    metadata: grpcWeb.Metadata | null,
    callback?: (err: grpcWeb.RpcError,
               response: Common_pb.CommonServiceResponse) => void) {
    if (callback !== undefined) {
      return this.client_.rpcCall(
        this.hostname_ +
          '/BepInEx.ModManagerService/AddPluginToRepo',
        request,
        metadata || {},
        this.methodDescriptorAddPluginToRepo,
        callback);
    }
    return this.client_.unaryCall(
    this.hostname_ +
      '/BepInEx.ModManagerService/AddPluginToRepo',
    request,
    metadata || {},
    this.methodDescriptorAddPluginToRepo);
  }

  methodDescriptorInstallPlugin = new grpcWeb.MethodDescriptor(
    '/BepInEx.ModManagerService/InstallPlugin',
    grpcWeb.MethodType.UNARY,
    Repo_pb.InstallPluginRequest,
    Common_pb.CommonServiceResponse,
    (request: Repo_pb.InstallPluginRequest) => {
      return request.serializeBinary();
    },
    Common_pb.CommonServiceResponse.deserializeBinary
  );

  installPlugin(
    request: Repo_pb.InstallPluginRequest,
    metadata: grpcWeb.Metadata | null): Promise<Common_pb.CommonServiceResponse>;

  installPlugin(
    request: Repo_pb.InstallPluginRequest,
    metadata: grpcWeb.Metadata | null,
    callback: (err: grpcWeb.RpcError,
               response: Common_pb.CommonServiceResponse) => void): grpcWeb.ClientReadableStream<Common_pb.CommonServiceResponse>;

  installPlugin(
    request: Repo_pb.InstallPluginRequest,
    metadata: grpcWeb.Metadata | null,
    callback?: (err: grpcWeb.RpcError,
               response: Common_pb.CommonServiceResponse) => void) {
    if (callback !== undefined) {
      return this.client_.rpcCall(
        this.hostname_ +
          '/BepInEx.ModManagerService/InstallPlugin',
        request,
        metadata || {},
        this.methodDescriptorInstallPlugin,
        callback);
    }
    return this.client_.unaryCall(
    this.hostname_ +
      '/BepInEx.ModManagerService/InstallPlugin',
    request,
    metadata || {},
    this.methodDescriptorInstallPlugin);
  }

  methodDescriptorUninstallPlugin = new grpcWeb.MethodDescriptor(
    '/BepInEx.ModManagerService/UninstallPlugin',
    grpcWeb.MethodType.UNARY,
    Repo_pb.UninstallPluginRequest,
    Common_pb.CommonServiceResponse,
    (request: Repo_pb.UninstallPluginRequest) => {
      return request.serializeBinary();
    },
    Common_pb.CommonServiceResponse.deserializeBinary
  );

  uninstallPlugin(
    request: Repo_pb.UninstallPluginRequest,
    metadata: grpcWeb.Metadata | null): Promise<Common_pb.CommonServiceResponse>;

  uninstallPlugin(
    request: Repo_pb.UninstallPluginRequest,
    metadata: grpcWeb.Metadata | null,
    callback: (err: grpcWeb.RpcError,
               response: Common_pb.CommonServiceResponse) => void): grpcWeb.ClientReadableStream<Common_pb.CommonServiceResponse>;

  uninstallPlugin(
    request: Repo_pb.UninstallPluginRequest,
    metadata: grpcWeb.Metadata | null,
    callback?: (err: grpcWeb.RpcError,
               response: Common_pb.CommonServiceResponse) => void) {
    if (callback !== undefined) {
      return this.client_.rpcCall(
        this.hostname_ +
          '/BepInEx.ModManagerService/UninstallPlugin',
        request,
        metadata || {},
        this.methodDescriptorUninstallPlugin,
        callback);
    }
    return this.client_.unaryCall(
    this.hostname_ +
      '/BepInEx.ModManagerService/UninstallPlugin',
    request,
    metadata || {},
    this.methodDescriptorUninstallPlugin);
  }

  methodDescriptorReadConfig = new grpcWeb.MethodDescriptor(
    '/BepInEx.ModManagerService/ReadConfig',
    grpcWeb.MethodType.UNARY,
    Repo_pb.ReadConfigRequest,
    Repo_pb.ReadConfigResponse,
    (request: Repo_pb.ReadConfigRequest) => {
      return request.serializeBinary();
    },
    Repo_pb.ReadConfigResponse.deserializeBinary
  );

  readConfig(
    request: Repo_pb.ReadConfigRequest,
    metadata: grpcWeb.Metadata | null): Promise<Repo_pb.ReadConfigResponse>;

  readConfig(
    request: Repo_pb.ReadConfigRequest,
    metadata: grpcWeb.Metadata | null,
    callback: (err: grpcWeb.RpcError,
               response: Repo_pb.ReadConfigResponse) => void): grpcWeb.ClientReadableStream<Repo_pb.ReadConfigResponse>;

  readConfig(
    request: Repo_pb.ReadConfigRequest,
    metadata: grpcWeb.Metadata | null,
    callback?: (err: grpcWeb.RpcError,
               response: Repo_pb.ReadConfigResponse) => void) {
    if (callback !== undefined) {
      return this.client_.rpcCall(
        this.hostname_ +
          '/BepInEx.ModManagerService/ReadConfig',
        request,
        metadata || {},
        this.methodDescriptorReadConfig,
        callback);
    }
    return this.client_.unaryCall(
    this.hostname_ +
      '/BepInEx.ModManagerService/ReadConfig',
    request,
    metadata || {},
    this.methodDescriptorReadConfig);
  }

  methodDescriptorWriteConfig = new grpcWeb.MethodDescriptor(
    '/BepInEx.ModManagerService/WriteConfig',
    grpcWeb.MethodType.UNARY,
    Repo_pb.WriteConfigRequest,
    Common_pb.CommonServiceResponse,
    (request: Repo_pb.WriteConfigRequest) => {
      return request.serializeBinary();
    },
    Common_pb.CommonServiceResponse.deserializeBinary
  );

  writeConfig(
    request: Repo_pb.WriteConfigRequest,
    metadata: grpcWeb.Metadata | null): Promise<Common_pb.CommonServiceResponse>;

  writeConfig(
    request: Repo_pb.WriteConfigRequest,
    metadata: grpcWeb.Metadata | null,
    callback: (err: grpcWeb.RpcError,
               response: Common_pb.CommonServiceResponse) => void): grpcWeb.ClientReadableStream<Common_pb.CommonServiceResponse>;

  writeConfig(
    request: Repo_pb.WriteConfigRequest,
    metadata: grpcWeb.Metadata | null,
    callback?: (err: grpcWeb.RpcError,
               response: Common_pb.CommonServiceResponse) => void) {
    if (callback !== undefined) {
      return this.client_.rpcCall(
        this.hostname_ +
          '/BepInEx.ModManagerService/WriteConfig',
        request,
        metadata || {},
        this.methodDescriptorWriteConfig,
        callback);
    }
    return this.client_.unaryCall(
    this.hostname_ +
      '/BepInEx.ModManagerService/WriteConfig',
    request,
    metadata || {},
    this.methodDescriptorWriteConfig);
  }

  methodDescriptorAddGame = new grpcWeb.MethodDescriptor(
    '/BepInEx.ModManagerService/AddGame',
    grpcWeb.MethodType.UNARY,
    Repo_pb.AddGameRequest,
    Common_pb.CommonServiceResponse,
    (request: Repo_pb.AddGameRequest) => {
      return request.serializeBinary();
    },
    Common_pb.CommonServiceResponse.deserializeBinary
  );

  addGame(
    request: Repo_pb.AddGameRequest,
    metadata: grpcWeb.Metadata | null): Promise<Common_pb.CommonServiceResponse>;

  addGame(
    request: Repo_pb.AddGameRequest,
    metadata: grpcWeb.Metadata | null,
    callback: (err: grpcWeb.RpcError,
               response: Common_pb.CommonServiceResponse) => void): grpcWeb.ClientReadableStream<Common_pb.CommonServiceResponse>;

  addGame(
    request: Repo_pb.AddGameRequest,
    metadata: grpcWeb.Metadata | null,
    callback?: (err: grpcWeb.RpcError,
               response: Common_pb.CommonServiceResponse) => void) {
    if (callback !== undefined) {
      return this.client_.rpcCall(
        this.hostname_ +
          '/BepInEx.ModManagerService/AddGame',
        request,
        metadata || {},
        this.methodDescriptorAddGame,
        callback);
    }
    return this.client_.unaryCall(
    this.hostname_ +
      '/BepInEx.ModManagerService/AddGame',
    request,
    metadata || {},
    this.methodDescriptorAddGame);
  }

  methodDescriptorLongConnect = new grpcWeb.MethodDescriptor(
    '/BepInEx.ModManagerService/LongConnect',
    grpcWeb.MethodType.SERVER_STREAMING,
    Service_pb.LongConnectRequest,
    Service_pb.LongConnectResponse,
    (request: Service_pb.LongConnectRequest) => {
      return request.serializeBinary();
    },
    Service_pb.LongConnectResponse.deserializeBinary
  );

  longConnect(
    request: Service_pb.LongConnectRequest,
    metadata?: grpcWeb.Metadata): grpcWeb.ClientReadableStream<Service_pb.LongConnectResponse> {
    return this.client_.serverStreaming(
      this.hostname_ +
        '/BepInEx.ModManagerService/LongConnect',
      request,
      metadata || {},
      this.methodDescriptorLongConnect);
  }

}

