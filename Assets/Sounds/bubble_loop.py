with_fx :reverb, mix: 0.1, room: 1 do
  live_loop :melody do
    use_synth :fm
    play scale(:Eb2, :minor_pentatonic, num_octaves: 1).choose, release: 0.2, amp: rand
    sleep 0.2
  end
end