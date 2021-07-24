<template>
    <div>
        <div class="top-pane">
            <h2>
                <a-button type="primary" size="small" @click="refreshGames(false)">
                    Refresh Game List ({{ games.length }})
                </a-button>
                <a-button type="primary" size="small" @click="refreshPluginRepo(false)">
                    Refresh Plugin List ({{ repoPlugins.length }})
                </a-button>
                <a-button type="primary" size="small" @click="checkPluginRepoUpdates">
                    Check Plugin Repo Updates
                </a-button>
                <a-button type="primary" size="small" @click="manageConfig"> Manage config </a-button>
                <a-button type="primary" size="small" @click="managePlugins"> Manage plugins </a-button>
                <a-upload
                    name="file"
                    :directory="true"
                    :multiple="false"
                    :file-list="[]"
                    :show-upload-list="false"
                    accept=".dll"
                    :beforeUpload="handleAddGameBeforeUpload"
                    @change="handleAddGameChange"
                >
                    <a-button type="primary" size="small"> Add a game </a-button>
                </a-upload>
            </h2>
            <ConfigEditorModal
                :show="showConfigEditor"
                @refreshGames="refreshGames"
                @close="showConfigEditor = false"
            ></ConfigEditorModal>
        </div>
        <div class="container">
            <div class="game-pane">
                <a-collapse default-active-key="0">
                    <a-collapse-panel v-for="(g, i) in games" :key="i">
                        <game-title slot="header" :game="g"></game-title>
                        <game-card :game="g" @refreshGames="refreshGames" @installPlugin="installPlugin"></game-card>
                    </a-collapse-panel>
                </a-collapse>
                <InstallPluginModal
                    :game="installPluginGame"
                    :plugins="repoPlugins"
                    @close="closeInstallPluginModal"
                    @refreshGames="refreshGames"
                    @refreshPluginRepo="refreshPluginRepo"
                ></InstallPluginModal>
            </div>
            <div class="log-pane">
                <!-- <h4>Log</h4> -->
                <textarea ref="logWindow" v-model="log" disabled="disabled"></textarea>
            </div>
        </div>
    </div>
</template>

<script lang="ts">
import { Component, Vue, Watch } from 'vue-property-decorator';
import i18next from 'i18next';
import _ from 'lodash';
import { ListGamesRequest, GameInfo, PluginInfo } from './generated/Game_pb';
import { grpcClient, longConnectStream } from './utils';
import GameCard from './components/GameCard.vue';
import GameTitle from './components/GameTitle.vue';
import ConfigEditorModal from './components/ConfigEditorModal.vue';
import InstallPluginModal from './components/InstallPluginModal.vue';
import { LongConnectResponse, ServerSideNotification } from './generated/Service_pb';
import { CheckPluginUpdatesRequest, ListPluginsRequest, AddGameRequest } from './generated/Repo_pb';
import { shell } from 'electron';
import path from 'path';
import fs from 'fs';

@Component({
    components: { GameCard, GameTitle, InstallPluginModal, ConfigEditorModal },
})
export default class AppPage extends Vue {
    log = '';
    games: GameInfo[] = [];
    repoPlugins: PluginInfo[] = [];
    installPluginGame: GameInfo | null = null;

    showConfigEditor = false;

    @Watch('games')
    onGameChanged(games: GameInfo[], oldGames: GameInfo[]) {
        if (this.installPluginGame) {
            games.forEach((g) => {
                if (g.getName() == this.installPluginGame?.getName()) {
                    this.installPluginGame = g;
                }
            });
        }
    }

    get appVersion() {
        return window.location.hash ? window.location.hash.substr(1) : '1.0.0';
    }

    get nodeVersion() {
        return process.versions.node;
    }

    get chromeVersion() {
        return process.versions.chrome;
    }

    get electronVersion() {
        return process.versions.electron;
    }

    get logWindow() {
        return this.$refs['logWindow'] as HTMLTextAreaElement;
    }

    get configDir() {
        return path.join((process.env as any).USERPROFILE, '.bierepo');
    }

    refreshGames(silent = true) {
        console.log('refreshGames');
        if (!silent) {
            this.$message.info('Please wait...');
        }
        const request = new ListGamesRequest();
        grpcClient.listGames(request, {}, (err, response) => {
            this.games = response.getGamesList();
            if (!silent) {
                this.$message.success('Done');
            }
        });
    }

    checkPluginRepoUpdates() {
        console.log('checkPluginRepoUpdates');
        this.$message.info('Please wait...');
        const request = new CheckPluginUpdatesRequest();
        grpcClient.checkPluginUpdates(request, {}, (err, response) => {
            if (response.getSuccess()) {
                this.refreshPluginRepo();
                this.$message.success('Done');
            }
        });
    }

    refreshPluginRepo(silent = true) {
        if (!silent) {
            this.$message.info('Please wait...');
        }
        console.log('refreshPluginRepo');
        const request = new ListPluginsRequest();
        grpcClient.listPlugins(request, {}, (err, response) => {
            this.repoPlugins = response.getPluginsList();
            if (!silent) {
                this.$message.success('Done');
            }
        });
    }

    installPlugin(game: GameInfo) {
        // console.log(`installPlugin - ${game.getName()}`);
        this.installPluginGame = game;
    }

    closeInstallPluginModal() {
        this.installPluginGame = null;
    }

    manageConfig() {
        // web editor instead
        this.showConfigEditor = true;
        // shell.openPath(this.configDir);
        // shell.openPath(path.join(this.configDir, 'config.yaml'));
    }

    managePlugins() {
        shell.openPath(path.join(this.configDir, 'plugins'));
    }

    handleAddGameBeforeUpload() {
        return false;
    }

    handleAddGameChange(info) {
        // console.log(info);
        const fileObj = info.file as File;
        if (fileObj && fileObj.name.toLowerCase() != 'UnityPlayer.dll'.toLowerCase()) {
            return;
        }
        const dir = path.dirname(fileObj.path);
        console.log(dir);
        const request = new AddGameRequest().setPath(dir);
        grpcClient.addGame(request, {}, (err, response) => {
            try {
                if (err) {
                    this.$message.error(err.message);
                } else if (response.getSuccess()) {
                    this.refreshGames();
                    this.$message.success('Done');
                } else {
                    this.$message.warn(response.getError());
                }
            } finally {
            }
        });
        return false;
    }

    todo() {
        this.$message.warning(`Not implemented yet!`);
    }

    created() {
        document.title = `${i18next.t('BepInEx Mod Manager')} ${this.appVersion}`;
        longConnectStream.on('data', (response) => {
            const notification = response.getNotification();
            if (notification == ServerSideNotification.REFRESHGAMEINFO) {
                this.refreshGames();
            } else if (notification == ServerSideNotification.REFRESHREPOINFO) {
                this.refreshPluginRepo();
            } else {
                const msg = response.getMessage();
                this.log = `${this.log}\n${i18next.t(msg)}`;
                const textarea = this.logWindow;
                // v-model refresh takes time, maybe set dom property directly instead.
                this.$nextTick().then(() => {
                    textarea.scrollTop = textarea.scrollHeight;
                });
                // setTimeout(() => {
                //     textarea.scrollTop = textarea.scrollHeight;
                // }, 100);
            }
        });

        this.refreshGames();
        this.refreshPluginRepo();
    }
}
</script>

<style lang="scss">
</style>

<style lang="scss" scoped>
.top-pane {
    padding-left: 1vw;
}
.container {
    display: flex;
    overflow: auto;
    height: 90vh;
    .game-pane {
        width: 70%;
    }
    .log-pane {
        width: 30%;
        height: 90vh;
        position: fixed;
        right: 0;
        overflow: hidden;
        padding-left: 1vw;
        textarea {
            width: 90%;
            height: 100%;
            line-height: 1.8em;
        }
    }
}
</style>
