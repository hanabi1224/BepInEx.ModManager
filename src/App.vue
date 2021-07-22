<template>
    <div>
        <h2>Steam Games:</h2>
        <div v-for="g in games">
            <h3>{{ g.getId() }} - {{ g.getName() }}</h3>
            <p>Path: {{ g.getPath() }}</p>
            <p>BIE installed: {{ g.getIsbieinstalled() }}, initialized {{ g.getIsbieinitialized() }}</p>
            <div v-if="g.getIsbieinstalled()">
                Installed plugins:
                <p v-for="p in g.getPluginsList()">{{ p.getName() }}</p>
                Installed patchers:
                <p v-for="p in g.getPatchersList()">{{ p.getName() }}</p>
            </div>
        </div>
    </div>
</template>

<script lang="ts">
import { Component, Vue } from 'vue-property-decorator';
import i18next from 'i18next';
import { ListSteamGamesRequest, GameInfo } from './generated/Steam_pb';
import { grpcClient } from './utils';

@Component({
    components: {},
})
export default class AppPage extends Vue {
    games: GameInfo[] = [];

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

    refreshSteamGames() {
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
