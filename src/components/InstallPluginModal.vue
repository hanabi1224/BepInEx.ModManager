<template>
  <div>
    <a-modal
      :title="title"
      :visible="!!game"
      :cancel-text="i18n('Close')"
      :ok-text="null"
      :ok-button-props="{ style: { display: 'none' } }"
      @ok="close"
      @cancel="close"
    >
      <a-spin
        v-if="!!game"
        :spinning="installing"
        size="large"
      >
        <div class="upload-zone">
          <a-upload-dragger
            name="file"
            accept=".dll,.zip"
            :multiple="false"
            :file-list="[]"
            @beforeUpload="handleFileBeforeUpload"
            @change="handleFileUpload"
          >
            <p class="ant-upload-drag-icon">
              <a-icon type="upload" />
            </p>
            <p class="ant-upload-text">
              {{ i18n('Upload') }}
            </p>
          </a-upload-dragger>
        </div>
        <a-input-group compact>
          <a-input
            v-model="filterText"
            style="width: 60%"
            placeholder="Search"
          ></a-input>
        </a-input-group>
        <div class="container">
          <a-list
            item-layout="horizontal"
            :data-source="filteredPlugins"
          >
            <a-list-item
              slot="renderItem"
              :key="getPluginKey(item)"
              slot-scope="item"
            >
              <a-list-item-meta :description="item.getName()">
                <span slot="title">{{ item.getId() }}(v{{ item.getVersion() }})</span>
              </a-list-item-meta>
              <p>{{ item.getPath() }}</p>
              Actions:
              <a-popconfirm
                :title="getConfirmInstallPluginMessage(item)"
                ok-text="Yes"
                cancel-text="No"
                @confirm="installPlugin(item)"
              >
                <a-button
                  type="primary"
                  size="small"
                >
                  Install
                </a-button>
              </a-popconfirm>
            </a-list-item>
          </a-list>
        </div>
      </a-spin>
    </a-modal>
  </div>
</template>

<script lang="ts">
import { Component, Vue, Prop, Watch } from 'vue-property-decorator';
import { GameInfo, PluginInfo } from '../generated/Game_pb';
import i18next from 'i18next';
import { Error } from 'grpc-web';
import _ from 'lodash';
import { AddPluginToRepoRequest, InstallPluginRequest } from '../generated/Repo_pb';
import { grpcClient } from '../utils';
import fs from 'fs';
import { CommonServiceResponse } from '../generated/Common_pb';

class PluginInfoExVersion {
    version!: string;
    path!: string;
}

class PluginInfoEx {
    id!: string;
    name!: string;
    installedVersion?: string;
    latestVersion!: string;
    versions!: PluginInfoExVersion[];
}

@Component({
    components: {},
})
export default class InstallPluginModal extends Vue {
    @Prop()
    plugins!: PluginInfo[];

    @Prop()
    game?: GameInfo;

    filterText = '';

    installing = false;

    get title() {
        return `(${this.game?.getName()}) ${i18next.t('Select a plugin')}`;
    }

    get filteredPlugins() {
        return _.chain(this.plugins)
            .filter((p) => {
                if (this.filterText) {
                    // TODO: Debounce if search perf is bad
                    const loweredFilterText = this.filterText.toLowerCase();
                    if (
                        p.getId().toLowerCase().indexOf(loweredFilterText) < 0 &&
                        p.getName().toLowerCase().indexOf(loweredFilterText)
                    ) {
                        return false;
                    }
                }
                const installed = this.game?.getPluginsList();
                if (installed) {
                    if (
                        _.chain(installed)
                            .findIndex((ip) => ip.getId() == p.getId() && ip.getVersion() == p.getVersion())
                            .value() >= 0
                    ) {
                        return false;
                    }
                }
                return true;
            })
            .orderBy((p) => p.getVersion(), 'desc')
            .orderBy((p) => p.getId())
            .value();
    }

    // Use this in the future
    get pluginsEx() {
        const groups = _.chain(this.plugins)
            .groupBy((p) => p.getId())
            .value();
        const pluginsEx: PluginInfoEx[] = [];
        for (const id in groups) {
            const piEx = new PluginInfoEx();
            const versions = groups[id];
            piEx.id = id;
            piEx.versions = [];
            let latestVersion = versions[0];
            versions.forEach((v) => {
                const versionEx = new PluginInfoExVersion();
                versionEx.version = v.getVersion();
                versionEx.path = v.getPath();
                piEx.versions.push(versionEx);
                if (versionEx.version > latestVersion.getVersion()) {
                    latestVersion = v;
                }
            });
            piEx.name = latestVersion.getName();
            piEx.latestVersion = latestVersion.getVersion();
            this.game?.getPluginsList()?.forEach((p) => {
                if (p.getId() == id) {
                    piEx.installedVersion = p.getVersion();
                }
            });
        }
        return pluginsEx;
    }

    getPluginKey(p: PluginInfo) {
        return `${p.getId()}-${p.getVersion()}`;
    }

    getConfirmInstallPluginMessage(p: PluginInfo) {
        return `Install ${p.getId()}(${p.getVersion()})?`;
    }

    handleCommonResponse(err: Error, response: CommonServiceResponse, callback) {
        try {
            if (err) {
                this.$message.error(err.message);
            } else if (response.getSuccess()) {
                if (callback) {
                    callback();
                }
                this.$message.success('Done');
            } else {
                this.$message.warn(response.getError());
            }
        } finally {
            this.installing = false;
        }
    }

    createCommonResponseHandler(callback) {
        return (err: Error, response: CommonServiceResponse) => {
            this.handleCommonResponse(err, response, callback);
        };
    }

    installPlugin(p: PluginInfo) {
        this.installing = true;
        const request = new InstallPluginRequest().setGamepath(this.game!.getPath()).setPluginpath(p.getPath());
        grpcClient.installPlugin(request, {}, this.createCommonResponseHandler(this.refreshGames));
    }

    handleFileBeforeUpload() {
        return false;
    }

    handleFileUpload(info) {
        const name = info.file.name as string;
        if (name.endsWith('.dll') || name.endsWith('.zip')) {
            console.log(info);
            const fileObj = info.file.originFileObj as File;
            this.$message.info('Uploading');
            const buffer = fs.readFileSync(fileObj.path);
            const request = new AddPluginToRepoRequest().setFilename(name).setData(buffer);
            console.log(request);
            grpcClient.addPluginToRepo(request, {}, this.createCommonResponseHandler(this.refreshPluginRepo));
        } else {
            this.$message.warning(i18next.t('Unsupported file type'));
        }
    }

    close() {
        this.$emit('close');
    }

    refreshGames() {
        this.$emit('refreshGames');
    }

    refreshPluginRepo() {
        this.$emit('refreshPluginRepo');
    }

    i18n(text) {
        return i18next.t(text);
    }

    todo() {
        this.$message.warning(`Not implemented yet!`);
    }
}
</script>

<style lang="scss">
// TODO: make style scoped
.ant-modal-content {
    position: fixed;
    left: 10vw;
    width: 80vw;
    top: 5vh;
    height: 90vh;
    overflow: auto;
}
</style>
<style lang="scss" scoped>
.upload-zone {
    position: fixed;
    right: 14vw;
    top: 12vh;
    width: 10vw;
    height: 10vh;
    z-index: 999;
}
.container {
    margin-top: 2vh;
    height: 65vh;
    overflow: auto;
}
</style>