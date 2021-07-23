<template>
    <div>
        <h2>Steam Games: <button @click="refreshSteamGames">Refresh</button></h2>
        <div v-for="g in games">
            <h3>{{ g.getId() }} - {{ g.getName() }} ({{ g.getIs64bit() ? '64bit' : '32bit' }})</h3>
            <p><button @click="openPath(g.getPath())">Open</button> Path: {{ g.getPath() }}</p>
            <p>
                <button v-if="!installing[g.getPath()] && !g.getIsbieinstalled()" @click="installBIE(g.getPath())">
                    Install BIE
                </button>
                <button v-if="!installing[g.getPath()] && g.getIsbieinstalled()" disabled="disabled">
                    BIE installed
                </button>
                <button v-if="installing[g.getPath()]" disabled="disabled">Installing</button>
                <!-- BIE installed: {{ g.getIsbieinstalled() }},
                initialized {{ g.getIsbieinitialized() }} -->
            </p>
            <div v-if="g.getIsbieinstalled()">
                Installed plugins:
                <p v-for="p in g.getPluginsList()">{{ p.getId() }}({{ p.getVersion() }}) - {{ p.getName() }}</p>
                Installed patchers:
                <p v-for="p in g.getPatchersList()">{{ p.getId() }}({{ p.getVersion() }}) - {{ p.getName() }}</p>
            </div>
        </div>
    </div>
</template>

<script lang="ts">
import { Component, Vue } from 'vue-property-decorator';
import i18next from 'i18next';
import { ListSteamGamesRequest, GameInfo, InstallBIERequest } from './generated/Steam_pb';
import { grpcClient } from './utils';
const { shell } = require('electron');

@Component({
    components: {},
})
export default class AppPage extends Vue {
    games: GameInfo[] = [];
    installing = {};

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

    openPath(path: string) {
        return shell.openPath(path);
    }

    installBIE(path: string) {
        this.installing[path] = true;
        this.refreshSteamGames();
        const request = new InstallBIERequest().setPath(path);
        grpcClient.installBIE(request, {}, (err, response) => {
            delete this.installing[path];
            if (response.getSuccess()) {
                this.refreshSteamGames();
            }
        });
    }

    refreshSteamGames() {
        // TODO: bounce
        const request = new ListSteamGamesRequest();
        grpcClient.listSteamGames(request, {}, (err, response) => {
            this.games = response.getGamesList();
        });
    }

    created() {
        document.title = `${i18next.t('BepInEx Mod Manager')} ${this.appVersion}`;
        this.refreshSteamGames();
    }
}
</script>

<style>
</style>
