<template>
  <div>
    <a-avatar
      v-if="game.getIcon()"
      :src="game.getIcon()"
    ></a-avatar>
    {{ game.getName() }} ({{ game.getIs64bit() ? '64bit' : '32bit' }}) - BIE
    <a-icon
      v-if="game.getIsbieinstalled()"
      type="check"
      style="color: green"
    ></a-icon>
    <a-icon
      v-if="!game.getIsbieinstalled()"
      type="close"
      style="color: red"
    ></a-icon>
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
export default class GameTitle extends Vue {
    @Prop()
    game!: GameInfo;

    openPath() {
        return shell.openPath(this.game.getPath());
    }
}
</script>

<style>
</style>
