<template>
  <div>
    <p>
      <a-button
        type="primary"
        size="small"
        @click="openPath()"
      >
        Open Game Folder
      </a-button>
      <a-popconfirm
        :title="confirmInstallBIEMessage"
        ok-text="Yes"
        cancel-text="No"
        @confirm="installBIE"
      >
        <a-button
          v-if="!pending && !game.getIsbieinstalled()"
          type="primary"
          size="small"
        >
          Install BIE
        </a-button>
      </a-popconfirm>
      <a-button
        v-if="!pending && game.getIsbieinstalled()"
        type="danger"
        size="small"
        @click="uninstallBIE()"
      >
        Uninstall BIE
      </a-button>
      <a-button
        v-if="pending"
        size="small"
        disabled="disabled"
      >
        Pending
      </a-button>
      <!-- BIE installed: {{ g.getIsbieinstalled() }},
                initialized {{ g.getIsbieinitialized() }} -->
    </p>
    <div v-if="game.getIsbieinstalled()">
      <div>
        Installed plugins:
        <p
          v-for="p in game.getPluginsList()"
          :key="p.getId()"
        >
          {{ p.getId() }}({{ p.getVersion() }}) - {{ p.getName() }}
        </p>
      </div>
      <div>
        Installed patchers:
        <p
          v-for="p in game.getPatchersList()"
          :key="p.getId()"
        >
          {{ p.getId() }}({{ p.getVersion() }}) - {{ p.getName() }}
        </p>
      </div>
    </div>
  </div>
</template>

<script lang="ts">
import { Component, Vue, Prop } from 'vue-property-decorator';
import i18next from 'i18next';
import { grpcClient } from './../utils';
import { GameInfo, InstallBIERequest } from './../generated/Steam_pb';
import { shell } from 'electron';

@Component({
    components: {},
})
export default class GameCard extends Vue {
    @Prop()
    game!: GameInfo;

    pending = false;

    openPath() {
        return shell.openPath(this.game.getPath());
    }

    get confirmInstallBIEMessage() {
        return `Install BIE for ${this.game.getName()}`;
    }

    installBIE() {
        this.pending = true;
        const request = new InstallBIERequest().setPath(this.game.getPath());
        grpcClient.installBIE(request, {}, (err, response) => {
            this.pending = false;
            if (response.getSuccess()) {
                this.refreshGames();
            }
        });
    }

    uninstallBIE() {
        this.$message.info(`Open game folder and then delete the BepInEx folder inside`);
    }

    refreshGames() {
        this.$emit('refreshGames');
    }
}
</script>

<style>
</style>
