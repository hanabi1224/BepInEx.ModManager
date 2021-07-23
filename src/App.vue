<template>
  <div>
    <div class="top-pane">
      <h2>
        <!-- {{ gameCount }} Games -->
        <a-button
          type="primary"
          size="small"
          @click="refreshGames"
        >
          Refresh Game List
        </a-button>
      </h2>
    </div>
    <div class="container">
      <div class="game-pane">
        <a-collapse default-active-key="0">
          <a-collapse-panel
            v-for="(g, i) in games"
            :key="i"
          >
            <game-title
              slot="header"
              :game="g"
            ></game-title>
            <game-card
              :game="g"
              @refreshGames="refreshGames"
            ></game-card>
          </a-collapse-panel>
        </a-collapse>
      </div>
      <div class="log-pane">
        <!-- <h4>Log</h4> -->
        <textarea
          v-model="log"
          disabled="disabled"
        ></textarea>
      </div>
    </div>
  </div>
</template>

<script lang="ts">
import { Component, Vue } from 'vue-property-decorator';
import i18next from 'i18next';
import _ from 'lodash';
import { ListSteamGamesRequest, GameInfo, InstallBIERequest } from './generated/Steam_pb';
import { grpcClient } from './utils';
import GameCard from './components/GameCard.vue';
import GameTitle from './components/GameTitle.vue';
const { shell } = require('electron');

@Component({
    components: { GameCard, GameTitle },
})
export default class AppPage extends Vue {
    log = '';
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
        this.refreshGames();
        const request = new InstallBIERequest().setPath(path);
        grpcClient.installBIE(request, {}, (err, response) => {
            delete this.installing[path];
            if (response.getSuccess()) {
                this.refreshGames();
            }
        });
    }

    refreshGames() {
        const request = new ListSteamGamesRequest();
        grpcClient.listSteamGames(request, {}, (err, response) => {
            this.games = response.getGamesList();
        });
    }

    created() {
        document.title = `${i18next.t('BepInEx Mod Manager')} ${this.appVersion}`;
        this.refreshGames();
    }
}
</script>

<style lang="scss">
// @import 'ant-design-vue/dist/antd.css';
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
        // height: 90vh;
        // position: fixed;
        // left: 0;
        // overflow: auto;
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
        }
    }
}
</style>
