<template>
    <div>
        <div class="top-pane">
            <h2>
                <!-- {{ gameCount }} Games -->
                <a-button type="primary" size="small" @click="refreshGames"> Refresh Game List </a-button>
                <a-button type="primary" size="small" @click="todo"> Manage games ({{ games.length }}) </a-button>
                <a-button type="primary" size="small" @click="todo">
                    Manage plugins ({{ repoPlugins.length }})
                </a-button>
                <a-button type="primary" size="small" @click="todo"> Manage plugin index </a-button>
            </h2>
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
import InstallPluginModal from './components/InstallPluginModal.vue';
import { LongConnectResponse, ServerSideNotification } from './generated/Service_pb';
import { ListPluginsRequest } from './generated/Repo_pb';
const { shell } = require('electron');

@Component({
    components: { GameCard, GameTitle, InstallPluginModal },
})
export default class AppPage extends Vue {
    log = '';
    games: GameInfo[] = [];
    repoPlugins: PluginInfo[] = [];
    installPluginGame: GameInfo | null = null;

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

    refreshGames() {
        console.log('refreshGames');
        const request = new ListGamesRequest();
        grpcClient.listGames(request, {}, (err, response) => {
            this.games = response.getGamesList();
        });
    }

    refreshPluginRepo() {
        console.log('refreshPluginRepo');
        const request = new ListPluginsRequest();
        grpcClient.listPlugins(request, {}, (err, response) => {
            this.repoPlugins = response.getPluginsList();
        });
    }

    installPlugin(game: GameInfo) {
        console.log(`installPlugin - ${game.getName()}`);
        this.installPluginGame = game;
    }

    closeInstallPluginModal() {
        this.installPluginGame = null;
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
